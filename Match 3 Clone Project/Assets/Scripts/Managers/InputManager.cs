using UnityEngine;

public class InputManager : Singleton<InputManager> 
{

    bool isInputEnabled;

    void Update()
    {
        if(isInputEnabled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool gotHit = Physics.Raycast(ray, out hitInfo, 50);

            if(gotHit)
            {
                BoardObject hitObject = hitInfo.collider.GetComponent<BoardObject>();
                if(hitObject != null)
                {
                    // Input has to be disabled until all the objects are settled on board
                    isInputEnabled = false;

                    BoardController.Instance.SetPositionLockOfAllBoardObjects(true);
                    hitObject.HandleClick();
                }
            }
        }
    }

    public void SetInputEnabled(bool isInputEnabled)
    {
        this.isInputEnabled = isInputEnabled;
    }
}
