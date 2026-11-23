// =====================================================================================
//  VERTEX COLOUR BRUSH  (editor-only tool - nothing here ships in your game build)
//
//  Paint vertex colours (and optionally flatten heights) directly on a mesh in the
//  Scene view. Built for the low-poly terrain meshes in this project, which use the
//  "Custom/LowPolyVertexColor" shader (the ground colour IS the vertex colour).
//
//  OPEN IT:   Tools > Vertex Color Brush
//  USE IT:    1. Select the terrain object (e.g. Terrain/Terrain_Continuous_LowPoly)
//             2. Click "Use selected object"
//             3. Press "Backup mesh" once (safe copy of the mesh)
//             4. Switch "Painting" ON, pick a colour, click-drag on the ground
//             5. Press "Save mesh" when happy, then save the scene (Ctrl+S)
//
//  SCENE VIEW CONTROLS (while Painting is ON)
//     Left-drag ........ paint          Ctrl+Click ... eyedropper (pick a colour from the ground)
//     Alt+drag ......... orbit camera   [ and ] ....... smaller / bigger brush
//     Ctrl+Z ........... undo a stroke
// =====================================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VertexColorBrush
{
    public enum BrushMode { PaintColour = 0, RestoreOriginal = 1, FlattenHeight = 2 }

    /// <summary>The maths of the brush, kept separate from the window so it is easy to test and reuse.</summary>
    public static class BrushMath
    {
        /// <summary>1 inside the "hard" core, smoothly fading to 0 at the brush edge.</summary>
        public static float Falloff(float t, float hardness)
        {
            if (t <= hardness) return 1f;
            float u = Mathf.Clamp01((t - hardness) / Mathf.Max(1f - hardness, 0.0001f));
            return 1f - u * u * (3f - 2f * u);
        }

        /// <summary>Blend vertex colours towards 'target' (or back to 'baseline' when restoring). Returns number of triangles/vertices touched.</summary>
        public static int ApplyColour(Vector3[] verts, int[] tris, Color[] cols, Color[] baseline, Vector3[] triCentroids,
                                      Vector3 hitLocal, float radiusLocal, float hardness, float strength,
                                      Color target, bool restore, bool wholeTriangles)
        {
            int changed = 0;
            if (wholeTriangles && triCentroids != null)
            {
                for (int t = 0; t < triCentroids.Length; t++)
                {
                    float d = (triCentroids[t] - hitLocal).magnitude;
                    if (d >= radiusLocal) continue;
                    float w = Falloff(d / radiusLocal, hardness) * strength;
                    if (w <= 0.0005f) continue;
                    for (int k = 0; k < 3; k++)
                    {
                        int idx = tris[t * 3 + k];
                        Color to = restore ? baseline[idx] : target;
                        cols[idx] = Color.Lerp(cols[idx], to, w);
                    }
                    changed++;
                }
            }
            else
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    float d = (verts[i] - hitLocal).magnitude;
                    if (d >= radiusLocal) continue;
                    float w = Falloff(d / radiusLocal, hardness) * strength;
                    if (w <= 0.0005f) continue;
                    Color to = restore ? baseline[i] : target;
                    cols[i] = Color.Lerp(cols[i], to, w);
                    changed++;
                }
            }
            return changed;
        }

        /// <summary>Pull vertex heights (local Y) towards the height under the brush centre. Returns vertices touched.</summary>
        public static int Flatten(Vector3[] verts, Vector3 hitLocal, float radiusLocal, float hardness, float strength)
        {
            int changed = 0;
            for (int i = 0; i < verts.Length; i++)
            {
                float dx = verts[i].x - hitLocal.x, dz = verts[i].z - hitLocal.z;
                float d = Mathf.Sqrt(dx * dx + dz * dz);
                if (d >= radiusLocal) continue;
                float w = Falloff(d / radiusLocal, hardness) * strength;
                if (w <= 0.0005f) continue;
                verts[i].y = Mathf.Lerp(verts[i].y, hitLocal.y, w);
                changed++;
            }
            return changed;
        }

        /// <summary>Ray vs. every triangle (Moller-Trumbore). Used when the object has no MeshCollider.</summary>
        public static bool RaycastMesh(Vector3[] verts, int[] tris, Ray ray, out float dist, out int tri)
        {
            dist = float.MaxValue; tri = -1;
            for (int t = 0; t < tris.Length / 3; t++)
            {
                Vector3 a = verts[tris[t * 3]], b = verts[tris[t * 3 + 1]], c = verts[tris[t * 3 + 2]];
                Vector3 e1 = b - a, e2 = c - a, p = Vector3.Cross(ray.direction, e2);
                float det = Vector3.Dot(e1, p);
                if (Mathf.Abs(det) < 1e-8f) continue;
                float inv = 1f / det; Vector3 s = ray.origin - a;
                float u = Vector3.Dot(s, p) * inv; if (u < 0f || u > 1f) continue;
                Vector3 q = Vector3.Cross(s, e1);
                float v = Vector3.Dot(ray.direction, q) * inv; if (v < 0f || u + v > 1f) continue;
                float d = Vector3.Dot(e2, q) * inv;
                if (d > 0f && d < dist) { dist = d; tri = t; }
            }
            return tri >= 0;
        }
    }

    public class VertexColorBrushWindow : EditorWindow
    {
        // ---- settings ----
        [SerializeField] GameObject target;
        BrushMode mode = BrushMode.PaintColour;
        Color brushColor = new Color(0.38f, 0.30f, 0.23f);   // forest dirt path (as you see it on screen)
        float radius = 3f;
        float hardness = 0.4f;
        float strength = 0.6f;
        bool wholeTriangles = true;       // keeps the flat low-poly look (one colour per triangle)
        bool convertToLinear = true;      // picker colour is sRGB; the mesh stores linear in a Linear project
        bool paintingActive;
        Vector2 scroll;

        // ---- cached mesh data ----
        MeshFilter mf; Mesh mesh; MeshCollider mcol;
        Vector3[] verts; int[] tris; Color[] cols; Color[] baseline; Vector3[] triCentroids;

        // ---- scene-view state ----
        bool hasHit; Vector3 hitPoint, hitNormal; bool strokeActive;

        static readonly KeyValuePair<string, Color>[] presets = new[]
        {
            new KeyValuePair<string, Color>("Forest dirt path", new Color(0.38f, 0.30f, 0.23f)),
            new KeyValuePair<string, Color>("Desert path",      new Color(0.96f, 0.80f, 0.58f)),
            new KeyValuePair<string, Color>("Dark soil",        new Color(0.20f, 0.16f, 0.15f)),
            new KeyValuePair<string, Color>("Deep green",       new Color(0.13f, 0.34f, 0.22f)),
            new KeyValuePair<string, Color>("Teal moss",        new Color(0.10f, 0.42f, 0.42f)),
            new KeyValuePair<string, Color>("Sand",             new Color(0.94f, 0.66f, 0.37f)),
            new KeyValuePair<string, Color>("Red-brown rock",   new Color(0.58f, 0.34f, 0.26f)),
        };

        [MenuItem("Tools/Vertex Color Brush")]
        public static void Open()
        {
            var w = GetWindow<VertexColorBrushWindow>("Vertex Brush");
            w.minSize = new Vector2(320, 420);
            w.Show();
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            Undo.undoRedoPerformed += OnUndoRedo;
            if (target == null && Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<MeshFilter>() != null)
                SetTarget(Selection.activeGameObject);
            else if (target != null) Refresh();
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedo;
            paintingActive = false;
        }

        // ------------------------------------------------------------------ target / cache
        void SetTarget(GameObject go) { target = go; Refresh(); }

        void Refresh()
        {
            mf = null; mesh = null; mcol = null; verts = null; tris = null; cols = null; baseline = null; triCentroids = null;
            if (target == null) return;
            mf = target.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return;
            mesh = mf.sharedMesh; mcol = target.GetComponent<MeshCollider>();
            if (!mesh.isReadable) return;
            CacheMesh(true);
        }

        void CacheMesh(bool takeBaseline)
        {
            if (mesh == null || !mesh.isReadable) return;
            verts = mesh.vertices; tris = mesh.triangles; cols = mesh.colors;
            if (cols == null || cols.Length != verts.Length)
            {
                cols = new Color[verts.Length];
                for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
            }
            if (takeBaseline || baseline == null || baseline.Length != cols.Length) baseline = (Color[])cols.Clone();
            BuildCentroids();
        }

        void BuildCentroids()
        {
            triCentroids = new Vector3[tris.Length / 3];
            for (int t = 0; t < triCentroids.Length; t++)
                triCentroids[t] = (verts[tris[t * 3]] + verts[tris[t * 3 + 1]] + verts[tris[t * 3 + 2]]) / 3f;
        }

        void OnUndoRedo() { if (mesh != null) { CacheMesh(false); RefreshCollider(); Repaint(); SceneView.RepaintAll(); } }

        void RefreshCollider() { if (mcol != null) { mcol.sharedMesh = null; mcol.sharedMesh = mesh; Physics.SyncTransforms(); } }

        Color ToStored(Color picked) => (convertToLinear && PlayerSettings.colorSpace == ColorSpace.Linear) ? picked.linear : picked;
        Color FromStored(Color stored) => (convertToLinear && PlayerSettings.colorSpace == ColorSpace.Linear) ? stored.gamma : stored;

        // ------------------------------------------------------------------ window UI
        void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.HelpBox("Paint vertex colours on a mesh in the Scene view.\n1) Select the terrain  2) Use selected object  3) Backup mesh  4) Painting ON, click-drag the ground.\nAlt+drag orbits, Ctrl+click is the eyedropper, [ ] resize the brush.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            var go = (GameObject)EditorGUILayout.ObjectField("Target object", target, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) SetTarget(go);
            if (GUILayout.Button("Use selected object")) { if (Selection.activeGameObject != null) SetTarget(Selection.activeGameObject); }

            // status
            if (target == null) EditorGUILayout.HelpBox("No target yet.", MessageType.Warning);
            else if (mesh == null) EditorGUILayout.HelpBox("The target needs a MeshFilter with a mesh.", MessageType.Error);
            else if (!mesh.isReadable) EditorGUILayout.HelpBox("This mesh is not readable (Read/Write is off in its import settings), so it can't be edited.", MessageType.Error);
            else
            {
                string path = AssetDatabase.GetAssetPath(mesh);
                EditorGUILayout.LabelField("Mesh", mesh.name + "  (" + mesh.vertexCount + " vertices, " + mesh.triangles.Length / 3 + " triangles)");
                if (string.IsNullOrEmpty(path))
                    EditorGUILayout.HelpBox("This mesh only lives inside the scene (it is not an asset). Use 'Save mesh' to turn it into an asset, or your edits can be lost.", MessageType.Warning);
                else EditorGUILayout.LabelField("Asset", path, EditorStyles.miniLabel);
                if (mcol == null) EditorGUILayout.HelpBox("No MeshCollider found - painting still works (slower aiming).", MessageType.None);
            }

            EditorGUILayout.Space();
            GUI.backgroundColor = paintingActive ? new Color(0.5f, 1f, 0.6f) : Color.white;
            bool newActive = GUILayout.Toggle(paintingActive, paintingActive ? "PAINTING: ON  (click-drag in the Scene view)" : "PAINTING: OFF  (click to enable)", "Button", GUILayout.Height(30));
            GUI.backgroundColor = Color.white;
            if (newActive != paintingActive) { paintingActive = newActive; SceneView.RepaintAll(); }

            mode = (BrushMode)GUILayout.Toolbar((int)mode, new[] { "Paint colour", "Restore original", "Flatten height" });
            if (mode == BrushMode.FlattenHeight)
                EditorGUILayout.HelpBox("Flatten pulls the ground under the brush to the height where you first click. Props on top will NOT follow - re-seat them afterwards (select, press End).", MessageType.Warning);
            else if (mode == BrushMode.RestoreOriginal)
                EditorGUILayout.HelpBox("Paints the colours back to how they were when the target was selected (or when you last pressed 'Set current colours as original').", MessageType.None);

            using (new EditorGUI.DisabledScope(mode != BrushMode.PaintColour))
            {
                brushColor = EditorGUILayout.ColorField(new GUIContent("Brush colour"), brushColor, true, false, false);
                EditorGUILayout.LabelField("Presets", EditorStyles.miniBoldLabel);
                int perRow = 2; int n = 0;
                foreach (var p in presets)
                {
                    if (n % perRow == 0) EditorGUILayout.BeginHorizontal();
                    var old = GUI.backgroundColor; GUI.backgroundColor = p.Value * 1.5f;
                    if (GUILayout.Button(p.Key)) brushColor = p.Value;
                    GUI.backgroundColor = old; n++;
                    if (n % perRow == 0) EditorGUILayout.EndHorizontal();
                }
                if (n % perRow != 0) EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            radius = EditorGUILayout.Slider("Radius (metres)", radius, 0.5f, 25f);
            hardness = EditorGUILayout.Slider("Hardness", hardness, 0f, 1f);
            strength = EditorGUILayout.Slider("Strength", strength, 0.05f, 1f);
            wholeTriangles = EditorGUILayout.ToggleLeft("Paint whole triangles (keeps the flat low-poly look)", wholeTriangles);
            convertToLinear = EditorGUILayout.ToggleLeft("Colour picker is sRGB -> convert for Linear colour space (leave on)", convertToLinear);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Safety", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(mesh == null))
            {
                if (GUILayout.Button("Backup mesh (makes a copy in Assets/VertexColorBrush_Backups)")) BackupMesh();
                if (GUILayout.Button("Set current colours as original (for Restore brush)")) { CacheMesh(true); }
                if (GUILayout.Button("Save mesh")) SaveMesh();
            }
            EditorGUILayout.HelpBox("Ctrl+Z undoes strokes. After painting: Save mesh, then save the scene.", MessageType.None);
            EditorGUILayout.EndScrollView();
        }

        void BackupMesh()
        {
            if (mesh == null) return;
            EnsureBackupFolder();
            var copy = Instantiate(mesh);
            copy.name = mesh.name + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string p = "Assets/VertexColorBrush_Backups/" + copy.name + ".asset";
            AssetDatabase.CreateAsset(copy, p);
            AssetDatabase.SaveAssets();
            Debug.Log("[Vertex Brush] Backup saved: " + p);
            EditorGUIUtility.PingObject(copy);
        }

        void SaveMesh()
        {
            if (mesh == null || target == null) return;
            string path = AssetDatabase.GetAssetPath(mesh);
            if (string.IsNullOrEmpty(path))
            {
                EnsureBackupFolder();
                string p = AssetDatabase.GenerateUniqueAssetPath("Assets/VertexColorBrush_Backups/" + mesh.name.Replace("/", "_") + "_Saved.asset");
                var copy = Instantiate(mesh); copy.name = System.IO.Path.GetFileNameWithoutExtension(p);
                AssetDatabase.CreateAsset(copy, p);
                Undo.RecordObject(mf, "Assign saved mesh"); mf.sharedMesh = copy;
                if (mcol != null) { Undo.RecordObject(mcol, "Assign saved mesh"); mcol.sharedMesh = copy; }
                mesh = copy; Debug.Log("[Vertex Brush] Mesh was in-scene only - saved as asset: " + p);
            }
            else { EditorUtility.SetDirty(mesh); }
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(target.scene);
            Debug.Log("[Vertex Brush] Mesh saved. Remember to save the scene (Ctrl+S).");
        }

        static void EnsureBackupFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/VertexColorBrush_Backups")) AssetDatabase.CreateFolder("Assets", "VertexColorBrush_Backups");
        }

        // ------------------------------------------------------------------ scene view
        bool RaycastTarget(Ray worldRay, out Vector3 point, out Vector3 normal)
        {
            point = Vector3.zero; normal = Vector3.up;
            if (mcol != null && mcol.enabled && mcol.sharedMesh == mesh)
            {
                RaycastHit h;
                if (mcol.Raycast(worldRay, out h, 100000f)) { point = h.point; normal = h.normal; return true; }
                return false;
            }
            Transform tr = target.transform;
            Ray local = new Ray(tr.InverseTransformPoint(worldRay.origin), tr.InverseTransformDirection(worldRay.direction));
            float d; int t;
            if (BrushMath.RaycastMesh(verts, tris, local, out d, out t))
            {
                Vector3 lp = local.origin + local.direction * d;
                point = tr.TransformPoint(lp);
                Vector3 n = Vector3.Cross(verts[tris[t * 3 + 1]] - verts[tris[t * 3]], verts[tris[t * 3 + 2]] - verts[tris[t * 3]]).normalized;
                normal = tr.TransformDirection(n); if (normal.y < 0) normal = -normal;
                return true;
            }
            return false;
        }

        void OnSceneGUI(SceneView sv)
        {
            if (!paintingActive || target == null || mesh == null || !mesh.isReadable || verts == null) return;
            Event e = Event.current;

            if (e.type == EventType.Layout) { HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); return; }
            if (e.alt && e.type != EventType.Repaint) return;     // let Alt+drag orbit the camera

            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.LeftBracket || e.keyCode == KeyCode.RightBracket))
            {
                radius = Mathf.Clamp(radius + (e.keyCode == KeyCode.RightBracket ? 0.5f : -0.5f), 0.5f, 25f);
                Repaint(); e.Use(); return;
            }

            if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
            {
                hasHit = RaycastTarget(HandleUtility.GUIPointToWorldRay(e.mousePosition), out hitPoint, out hitNormal);
                sv.Repaint();
            }

            if (e.type == EventType.MouseDown && e.button == 0 && hasHit)
            {
                if (e.control || e.command) { SampleColour(); }
                else { BeginStroke(); Apply(); }
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && e.button == 0 && strokeActive && hasHit) { Apply(); e.Use(); }
            else if (e.type == EventType.MouseUp && e.button == 0 && strokeActive) { EndStroke(); e.Use(); }

            if (e.type == EventType.Repaint)
            {
                if (hasHit)
                {
                    Color c = mode == BrushMode.PaintColour ? brushColor : (mode == BrushMode.FlattenHeight ? new Color(0.4f, 0.8f, 1f) : new Color(1f, 0.6f, 0.3f));
                    Handles.color = new Color(c.r, c.g, c.b, 0.22f); Handles.DrawSolidDisc(hitPoint, hitNormal, radius);
                    Handles.color = Color.white; Handles.DrawWireDisc(hitPoint, hitNormal, radius);
                    Handles.color = new Color(1, 1, 1, 0.5f); Handles.DrawWireDisc(hitPoint, hitNormal, radius * hardness);
                }
                Handles.BeginGUI();
                GUI.Label(new Rect(12, 10, 420, 22), "Vertex Brush: " + mode + "   radius " + radius.ToString("F1") + "   strength " + strength.ToString("F2") + "   ( [ ] resize | Ctrl+click eyedropper | Alt orbit )", EditorStyles.helpBox);
                Handles.EndGUI();
            }
        }

        void BeginStroke()
        {
            Undo.RegisterCompleteObjectUndo(mesh, "Vertex Colour Brush");
            Physics.SyncTransforms();
            strokeActive = true;
        }

        void EndStroke()
        {
            strokeActive = false;
            if (mode == BrushMode.FlattenHeight) RefreshCollider();
            EditorUtility.SetDirty(mesh);
            EditorSceneManager.MarkSceneDirty(target.scene);
        }

        void SampleColour()
        {
            Transform tr = target.transform; Vector3 hp = tr.InverseTransformPoint(hitPoint);
            int best = -1; float bd = float.MaxValue;
            for (int t = 0; t < triCentroids.Length; t++) { float d = (triCentroids[t] - hp).sqrMagnitude; if (d < bd) { bd = d; best = t; } }
            if (best >= 0) { Color stored = cols[tris[best * 3]]; Color g = FromStored(stored); g.a = 1f; brushColor = g; mode = BrushMode.PaintColour; Repaint(); }
        }

        void Apply()
        {
            if (!hasHit) return;
            Transform tr = target.transform;
            Vector3 hp = tr.InverseTransformPoint(hitPoint);
            float r = radius / Mathf.Max(tr.lossyScale.x, 0.0001f);

            if (mode == BrushMode.FlattenHeight)
            {
                if (BrushMath.Flatten(verts, hp, r, hardness, strength) > 0)
                {
                    mesh.vertices = verts; mesh.RecalculateNormals(); mesh.RecalculateBounds(); BuildCentroids();
                }
            }
            else
            {
                Color target4 = ToStored(brushColor); target4.a = 1f;
                if (BrushMath.ApplyColour(verts, tris, cols, baseline, triCentroids, hp, r, hardness, strength, target4, mode == BrushMode.RestoreOriginal, wholeTriangles) > 0)
                    mesh.colors = cols;
            }
            SceneView.RepaintAll();
        }
    }
}
