using FFM.Entity;
using FFM.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(EntityData), true)]
public class EntityDataInspector : Editor {

    internal void OnSceneDrag(SceneView sceneView, int index) {
        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();

                foreach (Object draggedObject in DragAndDrop.objectReferences) {
                    if (draggedObject is EntityData) {
                        Vector2 worldPosition = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition).GetPoint(0);

                        CreateEntity((EntityData)draggedObject, worldPosition);
                    }
                }
            }

            currentEvent.Use();
        }
    }

    private void CreateEntity(EntityData entityData, Vector2 position) {
        GameObject entityObject = new GameObject(entityData.name);
        entityObject.tag = entityData.EntityTag;
        entityObject.layer = entityData.EntityLayer.LayerIndex;
        entityObject.transform.position = position;

        EntityBase entityComponent = entityObject.AddComponent<EntityBase>();

        Rigidbody2D rb = entityComponent.GetComponent<Rigidbody2D>();
        rb.gravityScale = entityData.Gravity;
        rb.mass = entityData.Mass;
        rb.drag = entityData.Drag;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        entityComponent.GetComponents();
        entityComponent.SetData(entityData);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
