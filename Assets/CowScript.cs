using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowScript : MonoBehaviour
{

    private Vector3 _targetPos;
    [SerializeField] private bool _moveUp, _moveDown, _moveLeft, _moveRight;
    [SerializeField] public bool PlayerUp, PlayerDown, PlayerLeft, PlayerRight;
    [SerializeField] private float _walkingSpeed, _rotationSpeed;
    private bool _wallUp, _wallDown, _wallLeft, _wallRight;
    private Animator _anim;
    [SerializeField] private float _raycastRange = 0.8f;
    [SerializeField] private Vector3 _raycastOffset;

    private void Awake()
    {
        _anim = transform.GetChild(0).GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {    
        if (_moveUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90f, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.z == _targetPos.z)
            {
                _moveUp = false;
                _anim.SetBool("isWalking", false);
            }           
        }

        if (_moveDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 270f, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.z == _targetPos.z)
            {
                _moveDown = false;
                _anim.SetBool("isWalking", false);
            }
        }

        if (_moveLeft)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.x == _targetPos.x)
            {
                _moveLeft = false;
                _anim.SetBool("isWalking", false);
            }
        }

        if (_moveRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180f, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.x == _targetPos.x)
            {
                _moveRight = false;
                _anim.SetBool("isWalking", false);
            }
        }

        //////////////////////Collision//////////////////////
        Ray rayForward = new Ray(transform.position + _raycastOffset, Vector3.forward * _raycastRange);
        Ray rayDown = new Ray(transform.position + _raycastOffset, Vector3.back * _raycastRange);
        Ray rayLeft = new Ray(transform.position + _raycastOffset, Vector3.left * _raycastRange);
        Ray rayRight = new Ray(transform.position + _raycastOffset, Vector3.right * _raycastRange);

        Debug.DrawRay(transform.position + _raycastOffset, Vector3.forward * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.back * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.left * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.right * _raycastRange, Color.green);

        if (Physics.Raycast(rayForward, out RaycastHit hitForward, _raycastRange))
        {
            if (hitForward.collider.CompareTag("Walls"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.forward * _raycastRange, Color.red);
                _wallUp = true;
            }
            else if (hitForward.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.forward * _raycastRange, Color.yellow);
                PlayerUp = true;
            }
        }
        else
        {
           _wallUp = false;

            if (PlayerUp)
            {
                MoveUp();
                PlayerUp = false;
            }
            
           
        }

        if (Physics.Raycast(rayDown, out RaycastHit hitBack, _raycastRange))
        {
            if (hitBack.collider.CompareTag("Walls"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.back * _raycastRange, Color.red);
                _wallDown = true;
            }
            else if (hitBack.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.back * _raycastRange, Color.yellow);
                PlayerDown= true;
            }
        }
        else
        {
            _wallDown = false;

            if (PlayerDown)
            {
                MoveDown();
                PlayerDown = false;
            }

        }

        if (Physics.Raycast(rayLeft, out RaycastHit hitLeft, _raycastRange))
        {
            if (hitLeft.collider.CompareTag("Walls"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.left * _raycastRange, Color.red);
                _wallLeft = true;
            }
            else if (hitLeft.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.left * _raycastRange, Color.yellow);
                PlayerLeft = true;
            }
        }
        else
        {
                _wallLeft = false;

            if (PlayerLeft)
            {
                MoveLeft();
                PlayerLeft = false;
            }
        }

        if (Physics.Raycast(rayRight, out RaycastHit hitRight, _raycastRange))
        {
            if (hitRight.collider.CompareTag("Walls"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.right * _raycastRange, Color.red);
                _wallRight = true;
            }
            else if (hitRight.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.right * _raycastRange, Color.yellow);
                PlayerRight = true;
            }
        }
        else
        {
                _wallRight = false;

            if (PlayerRight)
            {
                MoveRight();
                PlayerRight = false;
            }
        }
    }

    public void MoveUp()
    {
        _targetPos = transform.position + new Vector3(0, 0, 1);
        _moveUp = true;
        _anim.SetBool("isWalking", true);
    }

    public void MoveDown()
    {
        _targetPos = transform.position + new Vector3(0, 0, -1);
        _moveDown = true;
        _anim.SetBool("isWalking", true);
    }
    public void MoveLeft()
    {
        _targetPos = transform.position + new Vector3(-1, 0, 0);
        _moveLeft = true;
        _anim.SetBool("isWalking", true);
    }

    public void MoveRight()
    {
        _targetPos = transform.position + new Vector3(1, 0, 0);
        _moveRight = true;
        _anim.SetBool("isWalking", true);
    }
}
