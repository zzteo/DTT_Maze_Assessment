using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private float _walkingSpeed, _rotationSpeed;
    [SerializeField] private bool _isMoving;
    private bool _isMovingUp, _isMovingDown, _isMovingLeft, _isMovingRight;
    [SerializeField] private bool _cantMoveUp, _cantMoveDown, _cantMoveLeft, _cantMoveRight;
    private Vector3 _targetPos;
    [SerializeField] private float _raycastRange = 0.8f;
    [SerializeField] private Vector3 _raycastOffset;
    private RaycastHit _hitForward;

    [SerializeField] private CowScript Cow;
    private BoxCollider _collider;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();
        _raycastOffset = new Vector3(0, 0.23f, 0);
        Cow = GameObject.FindGameObjectWithTag("Cow").GetComponent<CowScript>();
    }

    public void SpawnPlayer(Vector3 SpawnTransformPosition) 
    {   
        transform.position = SpawnTransformPosition;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            
            if (!_isMoving && !_cantMoveUp && Cow.PlayerDown)
            {
                _collider.enabled = false;
                Cow.MoveDown();
                _anim.SetTrigger("Jump");
                _targetPos = transform.position + new Vector3(0, 0, 1);
                _isMoving = true;
                _isMovingUp = true;
          
            }
            else if (!_isMoving && !_cantMoveUp)
            {              
                _targetPos = transform.position + new Vector3(0, 0, 1);
                _isMoving = true;
                _isMovingUp = true;
                _anim.SetBool("isWalking", true);
            }
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (!_isMoving && !_cantMoveDown && Cow.PlayerUp)
            {
                _collider.enabled = false;
                Cow.MoveUp();
                _anim.SetTrigger("Jump");
                _targetPos = transform.position + new Vector3(0, 0, -1);
                _isMoving = true;
                _isMovingDown = true;
                
            }
            else if (!_isMoving && !_cantMoveDown)
            {
                _targetPos = transform.position + new Vector3(0, 0, -1);
                _isMoving = true;
                _isMovingDown = true;
                _anim.SetBool("isWalking", true);
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!_isMoving && !_cantMoveLeft && Cow.PlayerRight)
            {
                Debug.Log("CRRRRRRR");
                _collider.enabled = false;
                Cow.MoveRight();
                _anim.SetTrigger("Jump");
                _targetPos = transform.position + new Vector3(-1, 0, 0);
                _isMoving = true;
                _isMovingLeft = true;
                
            }
            else if (!_isMoving && !_cantMoveLeft)
            {
                _targetPos = transform.position + new Vector3(-1, 0, 0);
                _isMoving = true;
                _isMovingLeft = true;
                _anim.SetBool("isWalking", true);
            }

        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!_isMoving && !_cantMoveRight && Cow.PlayerLeft)
            {
                Debug.Log("MMRRRRRRR");
                _collider.enabled = false;
                Cow.MoveLeft();
                _anim.SetTrigger("Jump");
                _targetPos = transform.position + new Vector3(1, 0, 0);
                _isMoving = true;
                _isMovingRight = true;
           
            }
            else if (!_isMoving && !_cantMoveRight)
            {       
                _targetPos = transform.position + new Vector3(1, 0, 0);
                _isMoving = true;
                _isMovingRight = true;
                _anim.SetBool("isWalking", true);
            }
        }

        if (_isMoving && _isMovingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.z == _targetPos.z)
            {
                _isMoving = false;
                _isMovingUp = false;
                _collider.enabled = true;

                if (!Input.anyKey)
                {
                    _anim.SetBool("isWalking", false);
                }
            }
        }

        if (_isMoving && _isMovingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.z == _targetPos.z)
            {
                _isMoving = false;
                _isMovingDown = false;
                _collider.enabled = true;

                if (!Input.anyKey)
                {
                    _anim.SetBool("isWalking", false);
                }
            }
        }

        if (_isMoving && _isMovingLeft)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.x == _targetPos.x)
            {
                _isMoving = false;
                _isMovingLeft = false;
                _collider.enabled = true;

                if (!Input.anyKey)
                {
                    _anim.SetBool("isWalking", false);
                }
            }
        }

        if (_isMoving && _isMovingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _walkingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), _rotationSpeed * Time.deltaTime);

            if (transform.position.x == _targetPos.x)
            {
                _isMoving = false;
                _isMovingRight = false;
                _collider.enabled = true;

                if (!Input.anyKey)
                {
                    _anim.SetBool("isWalking", false);
                }
            }
        }

        //////////////Collision//////////////////////

        Ray rayForward = new Ray(transform.position + _raycastOffset, Vector3.forward * _raycastRange);
        Ray rayDown = new Ray(transform.position + _raycastOffset, Vector3.back * _raycastRange);
        Ray rayLeft = new Ray(transform.position + _raycastOffset, Vector3.left * _raycastRange);
        Ray rayRight = new Ray(transform.position + _raycastOffset, Vector3.right * _raycastRange);

        Debug.DrawRay(transform.position + _raycastOffset, Vector3.forward * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.back * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.left * _raycastRange, Color.green);
        Debug.DrawRay(transform.position + _raycastOffset, Vector3.right * _raycastRange, Color.green);



        if (Physics.Raycast(rayForward, out _hitForward, _raycastRange))
        {
            if (_hitForward.collider.CompareTag("Walls"))
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.forward * _raycastRange, Color.red);
                _cantMoveUp = true;
            }          
        }
        else
        {
            _cantMoveUp = false;
        }

        if (Physics.Raycast(rayDown, out RaycastHit hitBack, _raycastRange))
        {
            if (hitBack.collider.tag == "Walls")
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.back * _raycastRange, Color.red);
                _cantMoveDown = true;
            }          
        }
        else
        {
            _cantMoveDown = false;
        }

        if (Physics.Raycast(rayLeft, out RaycastHit hitLeft, _raycastRange))
        {
            if (hitLeft.collider.tag == "Walls")
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.left * _raycastRange, Color.red);
                _cantMoveLeft = true;
            }
        }
        else
        {
            _cantMoveLeft = false;
        }

        if (Physics.Raycast(rayRight, out RaycastHit hitRight, _raycastRange))
        {
            if (hitRight.collider.tag == "Walls")
            {
                Debug.DrawRay(transform.position + _raycastOffset, Vector3.right * _raycastRange, Color.red);
                _cantMoveRight = true;
            }
        }
        else
        {
            _cantMoveRight = false;
        }

    }

}
