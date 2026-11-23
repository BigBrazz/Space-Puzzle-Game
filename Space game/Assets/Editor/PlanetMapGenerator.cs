// Unity 2023.1+ editor tool.
// Place in Assets/Editor/PlanetMapGenerator.cs
// Use Tools > Generate Planet Map > Generate Semi-Linear Planet Map.
// Creates a modular, semi-linear greybox. Existing scene objects are not modified.

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PlanetMapGenerator : EditorWindow
{
    private const string RootName = "GENERATED_PLANET_MAP - Semi Linear Greybox";
    private static Transform currentParent;
    private static Material groundMat, pathMat, rockMat, trunkMat, foliageMat, shipMat, campMat, puzzleMat, markerMat;

    [MenuItem("Tools/Generate Planet Map/Generate Semi-Linear Planet Map")]
    public static void Generate()
    {
        if (!EditorUtility.DisplayDialog("Generate Semi-Linear Planet Map",
            "This creates a new modular greybox level. Existing scene objects will be left untouched.\n\nIf a generated map with the same name exists, only that generated map will be replaced.",
            "Generate", "Cancel")) return;

        GameObject old = GameObject.Find(RootName);
        if (old != null) Undo.DestroyObjectImmediate(old);

        GameObject rootObject = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(rootObject, "Generate semi-linear planet map");
        currentParent = rootObject.transform;
        CreateMaterials();

        // 00 - Terrain and boundary
        BeginGroup("00 - TERRAIN & WORLD BOUNDARY");
        CreatePrimitive("Main ground - 120m x 240m", PrimitiveType.Cube, new Vector3(0, -1.1f, 0), new Vector3(120, 2, 240), groundMat);
        CreateBoundary();
        EndGroup();

        // 01 - Route is deliberately a sequence, not spokes from a central hub.
        BeginGroup("01 - MAIN ROUTE (FOLLOW NORTH)");
        CreatePath(new Vector3(0, 0.04f, -91), new Vector3(0, 0.04f, -65), "Route 1 - crash site to forest");
        CreatePath(new Vector3(0, 0.04f, -65), new Vector3(-7, 0.04f, -42), "Route 2 - forest to camp");
        CreatePath(new Vector3(-7, 0.04f, -42), new Vector3(4, 0.04f, -13), "Route 3 - camp to valley");
        CreatePath(new Vector3(4, 0.04f, -13), new Vector3(-4, 0.04f, 20), "Route 4 - valley to ruins");
        CreatePath(new Vector3(-4, 0.04f, 20), new Vector3(3, 0.04f, 53), "Route 5 - ruins to wreckage");
        CreatePath(new Vector3(3, 0.04f, 53), new Vector3(0, 0.04f, 91), "Route 6 - final approach");
        EndGroup();

        // AREA 1 - Crash site / start
        BeginGroup("02 - AREA 1 - CRASH SITE (START)");
        BeginGroup("Environment");
        CreatePrimitive("Crash clearing", PrimitiveType.Cylinder, new Vector3(0, 0.02f, -91), new Vector3(25, 0.25f, 22), pathMat);
        CreatePrimitive("Ship placeholder - replace with existing ship", PrimitiveType.Cube, new Vector3(0, 2.5f, -91), new Vector3(10, 5, 15), shipMat, new Vector3(0, 15, 0));
        ScatterRocks(-15, 15, -105, -77, 12);
        EndGroup();
        BeginGroup("Gameplay Markers");
        CreatePrimitive("PLAYER SPAWN - move existing Player here", PrimitiveType.Cylinder, new Vector3(0, 0.4f, -78), new Vector3(1.5f, 0.8f, 1.5f), markerMat);
        CreateSmallLabel("AREA 1 - CRASH SITE", new Vector3(0, 4.5f, -106));
        CreateObjectiveMarker("OXYGEN SUPPLY - starting area", new Vector3(-9, 0.9f, -84), puzzleMat);
        EndGroup();
        EndGroup();

        // AREA 2 - Forest / tree bridge
        BeginGroup("03 - AREA 2 - ALIEN FOREST");
        BeginGroup("Environment");
        ScatterTrees(-24, 24, -68, -43, 24);
        ScatterRocks(-24, 24, -68, -42, 12);
        CreatePrimitive("Forest ravine", PrimitiveType.Cube, new Vector3(0, -0.25f, -53), new Vector3(28, 0.6f, 7), rockMat);
        CreatePrimitive("Near bridge bank", PrimitiveType.Cube, new Vector3(0, 0.12f, -59), new Vector3(29, 0.45f, 5), groundMat);
        CreatePrimitive("Far bridge bank", PrimitiveType.Cube, new Vector3(0, 0.12f, -47), new Vector3(29, 0.45f, 5), groundMat);
        EndGroup();
        BeginGroup("Puzzle - Tree Bridge");
        CreatePrimitive("BROKEN BRIDGE - gap placeholder", PrimitiveType.Cube, new Vector3(0, 0.28f, -53), new Vector3(4, 0.5f, 7), campMat);
        CreatePrimitive("FALLEN TREE - visual solution placeholder", PrimitiveType.Cylinder, new Vector3(0, 0.55f, -53), new Vector3(1.1f, 7, 1.1f), trunkMat, new Vector3(90, 0, 90));
        CreateObjectiveMarker("AXE / CUTTING TOOL CLUE", new Vector3(-17, 0.8f, -61), puzzleMat);
        EndGroup();
        BeginGroup("Gameplay Markers");
        CreateObjectiveMarker("SHIP PART 1 - beyond bridge", new Vector3(0, 0.9f, -43), markerMat);
        CreateSmallLabel("AREA 2 - ALIEN FOREST", new Vector3(0, 5, -68));
        EndGroup();
        EndGroup();

        // AREA 3 - Survivor camp
        BeginGroup("04 - AREA 3 - SURVIVOR CAMP");
        BeginGroup("Environment");
        CreatePrimitive("Camp clearing", PrimitiveType.Cylinder, new Vector3(-7, 0.06f, -29), new Vector3(24, 0.25f, 19), pathMat);
        CreatePrimitive("Survivor shelter", PrimitiveType.Cube, new Vector3(-12, 2, -29), new Vector3(7, 4, 6), campMat);
        CreatePrimitive("Camp storage crate", PrimitiveType.Cube, new Vector3(-1, 0.8f, -33), new Vector3(2, 1.6f, 2), puzzleMat);
        ScatterTrees(-25, 12, -39, -20, 10);
        EndGroup();
        BeginGroup("NPC & Quest Markers");
        CreatePrimitive("NPC MARKER - survivor / engineer", PrimitiveType.Capsule, new Vector3(-6, 1.1f, -27), new Vector3(0.8f, 1.1f, 0.8f), markerMat);
        CreateObjectiveMarker("ENERGY CELL QUEST ITEM LOCATION", new Vector3(3, 0.8f, -24), puzzleMat);
        CreateSmallLabel("AREA 3 - SURVIVOR CAMP", new Vector3(-7, 5, -39));
        EndGroup();
        EndGroup();

        // AREA 4 - Rocky valley / enemy
        BeginGroup("05 - AREA 4 - ROCKY VALLEY");
        BeginGroup("Environment");
        ScatterRocks(-25, 25, -12, 17, 32);
        CreatePrimitive("Rocky hazard lane", PrimitiveType.Cube, new Vector3(4, -0.05f, 1), new Vector3(25, 0.35f, 9), rockMat);
        EndGroup();
        BeginGroup("Gameplay Markers");
        CreatePrimitive("ENEMY SPAWN MARKER", PrimitiveType.Capsule, new Vector3(7, 1.2f, 1), new Vector3(1.1f, 1.2f, 1.1f), campMat);
        CreateObjectiveMarker("SHIP PART 2", new Vector3(18, 0.9f, 12), markerMat);
        CreateObjectiveMarker("OXYGEN SUPPLY", new Vector3(-16, 0.8f, -4), puzzleMat);
        CreateSmallLabel("AREA 4 - ROCKY VALLEY", new Vector3(4, 5, -12));
        EndGroup();
        EndGroup();

        // AREA 5 - Ancient ruins / pressure plates
        BeginGroup("06 - AREA 5 - ANCIENT RUINS");
        BeginGroup("Environment");
        CreatePrimitive("Ruins platform", PrimitiveType.Cube, new Vector3(-4, 0.1f, 34), new Vector3(27, 0.5f, 19), pathMat);
        CreatePrimitive("Ruins left pillar", PrimitiveType.Cube, new Vector3(-15, 3.5f, 34), new Vector3(2, 7, 2), rockMat);
        CreatePrimitive("Ruins right pillar", PrimitiveType.Cube, new Vector3(7, 3.5f, 34), new Vector3(2, 7, 2), rockMat);
        CreatePrimitive("SEALED DOOR - placeholder", PrimitiveType.Cube, new Vector3(-4, 2.5f, 26), new Vector3(7, 5, 0.8f), campMat);
        ScatterRocks(-24, 17, 23, 47, 12);
        EndGroup();
        BeginGroup("Puzzle - Pressure Plates");
        CreatePrimitive("PRESSURE PLATE A", PrimitiveType.Cube, new Vector3(-9, 0.55f, 39), new Vector3(2.5f, 0.35f, 2.5f), puzzleMat);
        CreatePrimitive("PRESSURE PLATE B", PrimitiveType.Cube, new Vector3(1, 0.55f, 39), new Vector3(2.5f, 0.35f, 2.5f), puzzleMat);
        CreatePrimitive("MOVABLE CRATE", PrimitiveType.Cube, new Vector3(-19, 1, 43), new Vector3(2, 2, 2), campMat);
        EndGroup();
        BeginGroup("Gameplay Markers");
        CreateObjectiveMarker("SHIP PART 3 - behind sealed door", new Vector3(-4, 1, 21), markerMat);
        CreateSmallLabel("AREA 5 - ANCIENT RUINS", new Vector3(-4, 5.5f, 46));
        EndGroup();
        EndGroup();

        // AREA 6 - final wreckage zone
        BeginGroup("07 - AREA 6 - WRECKAGE ZONE (FINAL AREA)");
        BeginGroup("Environment");
        ScatterRocks(-24, 24, 63, 105, 24);
        CreatePrimitive("Old wreckage structure", PrimitiveType.Cube, new Vector3(0, 2, 87), new Vector3(15, 4, 9), shipMat, new Vector3(0, 0, 12));
        CreatePrimitive("Wreckage debris A", PrimitiveType.Cube, new Vector3(10, 0.8f, 79), new Vector3(5, 1.6f, 2), rockMat, new Vector3(0, 0, 24));
        CreatePrimitive("Wreckage debris B", PrimitiveType.Cube, new Vector3(-10, 0.8f, 94), new Vector3(4, 1.6f, 3), rockMat, new Vector3(0, 0, -18));
        EndGroup();
        BeginGroup("Gameplay Markers");
        CreateObjectiveMarker("SHIP PART 4", new Vector3(-12, 0.9f, 84), markerMat);
        CreateObjectiveMarker("SHIP PART 5 - final component", new Vector3(12, 0.9f, 96), markerMat);
        CreateObjectiveMarker("OXYGEN SUPPLY", new Vector3(-14, 0.8f, 72), puzzleMat);
        CreateSmallLabel("AREA 6 - WRECKAGE ZONE", new Vector3(0, 6, 105));
        EndGroup();
        EndGroup();

        AssetDatabase.SaveAssets();
        Selection.activeGameObject = rootObject;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Semi-linear planet greybox generated. Areas are grouped under the generated root. Move your existing Player and Ship to the markers, then replace placeholders with your assets.");
    }

    private static void CreateMaterials()
    {
        groundMat = MakeMaterial("MAP - muted alien ground", new Color(0.42f, 0.48f, 0.35f));
        pathMat = MakeMaterial("MAP - pale stone paths", new Color(0.63f, 0.59f, 0.43f));
        rockMat = MakeMaterial("MAP - dark rocks", new Color(0.28f, 0.31f, 0.29f));
        trunkMat = MakeMaterial("MAP - tree trunks", new Color(0.31f, 0.22f, 0.16f));
        foliageMat = MakeMaterial("MAP - alien foliage", new Color(0.34f, 0.53f, 0.43f));
        shipMat = MakeMaterial("MAP - crash site marker", new Color(0.45f, 0.55f, 0.68f));
        campMat = MakeMaterial("MAP - structures and hazards", new Color(0.68f, 0.48f, 0.30f));
        puzzleMat = MakeMaterial("MAP - puzzle and supply markers", new Color(0.75f, 0.62f, 0.30f));
        markerMat = MakeMaterial("MAP - objective markers", new Color(0.35f, 0.72f, 0.82f));
    }

    private static void BeginGroup(string groupName)
    {
        GameObject group = new GameObject(groupName);
        group.transform.SetParent(currentParent, false);
        Undo.RegisterCreatedObjectUndo(group, "Create map group");
        currentParent = group.transform;
    }

    private static void EndGroup()
    {
        if (currentParent != null && currentParent.parent != null)
            currentParent = currentParent.parent;
    }

    private static Material MakeMaterial(string materialName, Color color)
    {
        const string folder = "Assets/GeneratedPlanetMaterials";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "GeneratedPlanetMaterials");
        string path = folder + "/" + materialName.Replace("/", "-") + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) { existing.color = color; EditorUtility.SetDirty(existing); return existing; }
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material mat = new Material(shader) { name = materialName, color = color };
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static GameObject CreatePrimitive(string objectName, PrimitiveType type, Vector3 position, Vector3 scale, Material material, Vector3? rotation = null)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = objectName;
        obj.transform.SetParent(currentParent, true);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        if (rotation.HasValue) obj.transform.eulerAngles = rotation.Value;
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = material;
        Undo.RegisterCreatedObjectUndo(obj, "Create map object");
        return obj;
    }

    private static void CreateSmallLabel(string label, Vector3 position)
    {
        GameObject obj = new GameObject(label + " - editor label");
        obj.transform.SetParent(currentParent, true);
        obj.transform.position = position;
        TextMesh text = obj.AddComponent<TextMesh>();
        text.text = label;
        text.characterSize = 0.35f;
        text.fontSize = 24;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = new Color(1f, 1f, 1f, 0.8f);
        Undo.RegisterCreatedObjectUndo(obj, "Create map label");
    }

    private static void CreateObjectiveMarker(string label, Vector3 position, Material material)
    {
        CreatePrimitive(label, PrimitiveType.Sphere, position, Vector3.one * 1.0f, material);
        CreateSmallLabel(label, position + Vector3.up * 1.35f);
    }

    private static void CreatePath(Vector3 start, Vector3 end, string name)
    {
        Vector3 midpoint = (start + end) * 0.5f;
        Vector3 direction = end - start;
        GameObject path = CreatePrimitive(name, PrimitiveType.Cube, midpoint, new Vector3(4.5f, 0.12f, direction.magnitude), pathMat);
        path.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    private static void CreateBoundary()
    {
        CreatePrimitive("West boundary ridge", PrimitiveType.Cube, new Vector3(-59, 3, 0), new Vector3(4, 7, 240), rockMat);
        CreatePrimitive("East boundary ridge", PrimitiveType.Cube, new Vector3(59, 3, 0), new Vector3(4, 7, 240), rockMat);
        CreatePrimitive("North boundary ridge", PrimitiveType.Cube, new Vector3(0, 3, 119), new Vector3(120, 7, 4), rockMat);
        CreatePrimitive("South boundary ridge", PrimitiveType.Cube, new Vector3(0, 3, -119), new Vector3(120, 7, 4), rockMat);
    }

    private static void ScatterTrees(float minX, float maxX, float minZ, float maxZ, int count)
    {
        BeginGroup("Trees");
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(minX, maxX), z = Random.Range(minZ, maxZ), height = Random.Range(4f, 7f);
            CreatePrimitive("Tree trunk", PrimitiveType.Cylinder, new Vector3(x, height * 0.5f, z), new Vector3(0.65f, height * 0.5f, 0.65f), trunkMat);
            CreatePrimitive("Tree canopy", PrimitiveType.Sphere, new Vector3(x, height + 1.2f, z), new Vector3(3.2f, 3.5f, 3.2f), foliageMat);
        }
        EndGroup();
    }

    private static void ScatterRocks(float minX, float maxX, float minZ, float maxZ, int count)
    {
        BeginGroup("Rocks");
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(minX, maxX), z = Random.Range(minZ, maxZ), size = Random.Range(1.5f, 4f);
            GameObject rock = CreatePrimitive("Alien rock", PrimitiveType.Sphere, new Vector3(x, size * 0.35f, z), new Vector3(size * 1.3f, size * 0.7f, size), rockMat);
            rock.transform.rotation = Quaternion.Euler(Random.Range(-15, 15), Random.Range(0, 360), Random.Range(-15, 15));
        }
        EndGroup();
    }
}
