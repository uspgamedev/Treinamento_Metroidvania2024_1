using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLooping : MonoBehaviour
{
    private enum LoopAxis{
        X,
        Y,
    }
    [SerializeField] private GameObject loopingObject;
    [SerializeField] private float LOOP_SPEED = 1f;
    [SerializeField] private LoopAxis axisToLoop = LoopAxis.Y;
    [SerializeField] private Vector2 loopBoundary; //Só pra poder deixar bem evidente o limite em cada eixo, mesmo que não vá usar os dois limites ao mesmo tempo.
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = loopingObject.transform.position;    
    }
    void FixedUpdate()
    {
        loopingObject.transform.position = updatePosition(loopingObject.transform.position);
    }

    private Vector3 updatePosition(Vector3 vector){
        switch(axisToLoop){
            case LoopAxis.Y:
                if (vector.y <= loopBoundary.y){
                    vector.y+=LOOP_SPEED;
                    return vector;
                }
                break;
            case LoopAxis.X:
                if (vector.x >= loopBoundary.x){
                    vector.x+=LOOP_SPEED;
                    return vector;
                }
                break;
        }
        return initialPosition;

    }
}
