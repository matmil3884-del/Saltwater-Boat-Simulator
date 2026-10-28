using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class SaltwaterBuild
{
    const string ScenePath = "Assets/Scenes/SaltwaterPrototype.unity";

    [MenuItem("Saltwater/Create Prototype Scene")]
    public static void CreatePrototypeScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Water: simple blue surface for the first playable blockout.
        var ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ocean.name = "Ocean Surface - placeholder";
        ocean.transform.position = Vector3.zero;
        ocean.transform.localScale = new Vector3(180f, 1f, 180f);
        SetMaterial(ocean, new Color(0.025f, 0.24f, 0.36f));
        Object.DestroyImmediate(ocean.GetComponent<Collider>());

        // Distant islands made from primitive shapes, intentionally replaceable with proper assets.
        MakeIsland(new Vector3(28f, -0.6f, 38f), new Vector3(22f, 1.2f, 18f));
        MakeIsland(new Vector3(-45f, -0.7f, 70f), new Vector3(28f, 1.4f, 25f));
        MakeIsland(new Vector3(-60f, -0.8f, -35f), new Vector3(25f, 1.6f, 22f));

        var boat = new GameObject("PlayerBoat");
        boat.transform.position = new Vector3(0f, 0.2f, 0f);
        var hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.name = "Hull";
        hull.transform.SetParent(boat.transform, false);
        hull.transform.localScale = new Vector3(2.2f, 0.65f, 4.8f);
        SetMaterial(hull, new Color(0.92f, 0.92f, 0.86f));
        var body = boat.AddComponent<Rigidbody>();
        body.mass = 1000f;
        body.drag = 0.4f;
        body.angularDrag = 1.5f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        var controller = boat.AddComponent<BoatController>();
        var buoyancy = boat.AddComponent<BoatBuoyancy>();
        buoyancy.waterLevel = 0f;
        buoyancy.buoyancyStrength = 16f;
        buoyancy.buoyancyPoints = new Transform[4];
        Vector3[] offsets = {
            new Vector3(-0.8f, -0.28f, -1.7f), new Vector3(0.8f, -0.28f, -1.7f),
            new Vector3(-0.8f, -0.28f, 1.7f), new Vector3(0.8f, -0.28f, 1.7f)
        };
        for (int i = 0; i < offsets.Length; i++)
        {
            var p = new GameObject("FloatPoint" + i);
            p.transform.SetParent(boat.transform, false);
            p.transform.localPosition = offsets[i];
            buoyancy.buoyancyPoints[i] = p.transform;
        }

        // Cabin and windshield are simple placeholder geometry.
        var cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.name = "Cabin placeholder";
        cabin.transform.SetParent(boat.transform, false);
        cabin.transform.localPosition = new Vector3(0f, 0.52f, 0.25f);
        cabin.transform.localScale = new Vector3(1.25f, 0.72f, 1.25f);
        SetMaterial(cabin, new Color(0.08f, 0.16f, 0.2f));

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        var cam = cameraObject.AddComponent<Camera>();
        cam.fieldOfView = 62f;
        cameraObject.AddComponent<AudioListener>();
        var follow = cameraObject.AddComponent<BoatCameraFollow>();
        follow.target = boat.transform;
        cameraObject.transform.position = new Vector3(0f, 6f, -12f);

        var lightObject = new GameObject("Sun");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        RenderSettings.ambientLight = new Color(0.48f, 0.54f, 0.6f);
        Camera.main.backgroundColor = new Color(0.35f, 0.66f, 0.82f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        var manager = new GameObject("GameManager");
        manager.AddComponent<BoatGameManager>();

        // Fishing gameplay is part of the default generated scene.
        var fishingObject = new GameObject("FishingSystem");
        var fishing = fishingObject.AddComponent<FishingSystem>();
        fishing.boat = boat.transform;

        CreateBuoy(new Vector3(0f, 0.8f, 28f), "Harbor Checkpoint", 250, new Color(1f, 0.72f, 0.12f));
        CreateBuoy(new Vector3(22f, 0.8f, 52f), "Coastal Mission", 400, new Color(0.1f, 1f, 0.55f));
        var explore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        explore.name = "Hidden Cove Discovery Trigger";
        explore.transform.position = new Vector3(-42f, 1f, 62f);
        explore.transform.localScale = new Vector3(8f, 3f, 8f);
        explore.GetComponent<Collider>().isTrigger = true;
        SetMaterial(explore, new Color(0.2f, 0.9f, 0.65f, 0.35f));
        explore.AddComponent<ExplorationZone>();
        Object.DestroyImmediate(explore.GetComponent<MeshRenderer>());

        CreateMobileUI(boat);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        Debug.Log("Saltwater prototype scene created at " + ScenePath);
    }

    static void MakeIsland(Vector3 position, Vector3 scale)
    {
        var island = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        island.name = "Distant Island Placeholder";
        island.transform.position = position;
        island.transform.localScale = scale;
        SetMaterial(island, new Color(0.23f, 0.38f, 0.19f));
    }

    static void CreateBuoy(Vector3 position, string title, int reward, Color color)
    {
        var buoy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        buoy.name = title;
        buoy.transform.position = position;
        buoy.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        buoy.GetComponent<Collider>().isTrigger = true;
        SetMaterial(buoy, color);
        buoy.AddComponent<MissionBuoy>().rewardCredits = reward;
    }

    static void CreateMobileUI(GameObject boat)
    {
        var canvasObject = new GameObject("Mobile Controls Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        var eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        var controlsObject = new GameObject("MobileBoatControls");
        var controls = controlsObject.AddComponent<MobileBoatControls>();
        controls.boat = boat.GetComponent<BoatController>();
        MakeButton(canvasObject.transform, controls, "LEFT", HoldButton.HoldAction.Left, new Vector2(0.10f, 0.16f));
        MakeButton(canvasObject.transform, controls, "RIGHT", HoldButton.HoldAction.Right, new Vector2(0.22f, 0.16f));
        MakeButton(canvasObject.transform, controls, "THROTTLE", HoldButton.HoldAction.Throttle, new Vector2(0.87f, 0.20f));
        MakeButton(canvasObject.transform, controls, "REVERSE", HoldButton.HoldAction.Reverse, new Vector2(0.87f, 0.08f));
    }

    static void MakeButton(Transform parent, MobileBoatControls controls, string label, HoldButton.HoldAction action, Vector2 anchor)
    {
        var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(HoldButton));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero; rt.sizeDelta = new Vector2(190f, 120f);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.02f, 0.1f, 0.16f, 0.72f);
        var hold = go.GetComponent<HoldButton>();
        hold.controls = controls; hold.action = action;
        var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(go.transform, false);
        var tr = textGo.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
        var text = textGo.GetComponent<Text>();
        text.text = label; text.alignment = TextAnchor.MiddleCenter; text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 26; text.color = Color.white;
    }

    static void SetMaterial(GameObject go, Color color)
    {
        var renderer = go.GetComponent<Renderer>();
        if (!renderer) return;
        var mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        renderer.sharedMaterial = mat;
    }

    public static void BuildAndroid()
    {
        CreatePrototypeScene();
        PlayerSettings.productName = "Saltwater Fishing Simulator";
        PlayerSettings.companyName = "Saltwater Studio";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.saltwaterstudio.boatsimulator");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        Directory.CreateDirectory("BuildOutput");
        var options = new BuildPlayerOptions {
            scenes = new[] { ScenePath },
            locationPathName = "BuildOutput/SaltwaterFishingSimulator.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result);
        Debug.Log("APK build succeeded: " + options.locationPathName);
    }
}
