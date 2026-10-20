using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ShipPartHierarchyBuilder
{
    [MenuItem("Tools/Alien Quest/Create Ship-Part Visuals")]
    private static void CreateShipPartVisuals()
    {
        // Remove the incomplete primitive left behind if an earlier build was interrupted.
        GameObject incompletePrimitive = GameObject.Find("Cylinder");
        if (incompletePrimitive != null && incompletePrimitive.GetComponent<SpaceShipParts>() == null)
            Object.DestroyImmediate(incompletePrimitive);

        SpaceShipParts[] parts = Object.FindObjectsByType<SpaceShipParts>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SpaceShipParts part in parts)
        {
            Transform root = part.transform;
            ClearOldVisuals(root);

            MeshRenderer placeholder = root.GetComponent<MeshRenderer>();
            if (placeholder != null)
                placeholder.enabled = false;

            int index = GetIndex(root.name);
            if (index == 0) CreateWing(root);
            else if (index == 1) CreateWheel(root);
            else if (index == 2) CreateEngine(root);
            else if (index == 3) CreateCockpit(root);
            else CreateSolarPanel(root);
        }

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Created ship-part visuals in the hierarchy.");
    }

    private static void ClearOldVisuals(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(root.GetChild(i).gameObject);
    }

    private static int GetIndex(string objectName)
    {
        if (objectName == "SpaceshipPart") return 0;
        int open = objectName.LastIndexOf('(');
        int close = objectName.LastIndexOf(')');
        return open >= 0 && close > open && int.TryParse(objectName.Substring(open + 1, close - open - 1), out int index) ? index : 4;
    }

    private static void CreateWing(Transform root)
    {
        CreatePiece(root, "Wing", PrimitiveType.Cube, Vector3.zero, new Vector3(2.4f, 0.15f, 0.8f), Color.cyan);
        CreatePiece(root, "Wing Tip", PrimitiveType.Cube, new Vector3(1.05f, 0.12f, 0f), new Vector3(0.35f, 0.3f, 0.55f), Color.yellow);
    }

    private static void CreateWheel(Transform root)
    {
        CreatePiece(root, "Landing Tyre", PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.65f, 0.22f, 0.65f), Color.black, new Vector3(90f, 0f, 0f));
        CreatePiece(root, "Wheel Hub", PrimitiveType.Cylinder, new Vector3(0f, 0f, -0.24f), new Vector3(0.25f, 0.24f, 0.25f), Color.yellow, new Vector3(90f, 0f, 0f));
    }

    private static void CreateEngine(Transform root)
    {
        CreatePiece(root, "Engine", PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.55f, 0.85f, 0.55f), new Color(0.7f, 0.7f, 0.75f), new Vector3(90f, 0f, 0f));
        CreatePiece(root, "Engine Nozzle", PrimitiveType.Cylinder, new Vector3(0f, 0f, -0.55f), new Vector3(0.36f, 0.25f, 0.36f), new Color(0.2f, 0.2f, 0.25f), new Vector3(90f, 0f, 0f));
    }

    private static void CreateCockpit(Transform root)
    {
        CreatePiece(root, "Cockpit", PrimitiveType.Sphere, Vector3.zero, new Vector3(1.1f, 0.6f, 0.9f), new Color(0.15f, 0.65f, 1f));
        CreatePiece(root, "Cockpit Frame", PrimitiveType.Cube, new Vector3(0f, -0.35f, 0f), new Vector3(1.2f, 0.15f, 1f), new Color(0.65f, 0.65f, 0.7f));
    }

    private static void CreateSolarPanel(Transform root)
    {
        CreatePiece(root, "Solar Panel", PrimitiveType.Cube, Vector3.zero, new Vector3(1.8f, 0.08f, 0.8f), new Color(0.05f, 0.18f, 0.45f));
        CreatePiece(root, "Solar Panel Frame", PrimitiveType.Cube, new Vector3(0f, -0.08f, 0f), new Vector3(1.95f, 0.08f, 0.95f), new Color(0.65f, 0.65f, 0.7f));
    }

    private static void CreatePiece(Transform parent, string pieceName, PrimitiveType primitive, Vector3 localPosition, Vector3 localScale, Color colour, Vector3 localEulerAngles = default)
    {
        GameObject piece = GameObject.CreatePrimitive(primitive);
        piece.name = pieceName;
        piece.transform.SetParent(parent, false);
        piece.transform.localPosition = localPosition;
        piece.transform.localEulerAngles = localEulerAngles;
        piece.transform.localScale = localScale;
        Object.DestroyImmediate(piece.GetComponent<Collider>());
        // Keep Unity's existing default material. The permanently saved mesh shapes
        // and their hierarchy labels make each collectible recognisable without
        // introducing a render-pipeline-specific material dependency.
    }
}
