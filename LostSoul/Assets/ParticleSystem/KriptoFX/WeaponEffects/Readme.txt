version 1.0.0.0

NOTE: Demo worked with bloom and HDR. For corrected bloom with "Forward" mode disable multisampling, or use "Deffered" mode.  "Edit->Project Settings->Quality->Multisampling" 

Mesh effects works on mobile / PC / consoles with vertexlit / forward / deferred renderer and dx9, dx11, openGL. 
All effects optimized for mobile and pc. For mobile use optimized prefabs with optimized shaders.
NOTE: Mobile distortions work correctly only with script "WFX_MobileDistortion.cs". Just add script to any object (for example camera). 
 

Effect using:
1) Just drag&drop prefab on scene.
2) Set the "Mesh Object" of script "PSMeshRendererUpdater".
3) Click "Update Mesh Renderer".


For creating effect in runtime, just use follow code: 

var currentInstance = Instantiate(Effect, position, new Quaternion()) as GameObject; 
var psUpdater = currentInstance.GetComponent<PSMeshRendererUpdater>();
psUpdater.UpdateMeshEffect(MeshObject);


You can change scale of effect using tranform scale of gameObject. 



If you have some questions, you can write me to email "kripto289@gmail.com" 