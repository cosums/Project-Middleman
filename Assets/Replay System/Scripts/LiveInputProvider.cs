using UnityEngine;

public class LiveInputProvider : MonoBehaviour, IInputProvider
{
    public bool IsFinished => throw new System.NotImplementedException();

    public InputFrame GetInput(int tick)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
