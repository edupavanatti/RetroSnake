using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSnakeBehavior : MonoBehaviour
{
    private readonly List<Transform> SnakeSegments = new List<Transform>();
    private readonly Vector3 SpawnPosition = Vector3.zero;

    private const string FoodTag = "Food";
    private const string SnakeTag = "Snake";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    private const float NormalFoodTime = 0.02f;
    private const float SpecialFoodTime = -0.04f;
    private const float DefaultTimeToMove = 0.2f;

    [SerializeField] private Transform snakeHead;
    [SerializeField] private Transform snakeBody;

    private Vector3 _direction;
    private Vector3 _lastPosition;

    private FoodType _currentFoodType;
    private float _timeToMove = DefaultTimeToMove;
    private bool _isMovingVertically;
    private bool _canMove;
    private int _extraBlocks;

    private Action FoodEaten;
    private Action SnakeDied;
    private Action<int> LifeGained;
    private Action<int> LifeLost;

    private void OnEnable()
    {
        _direction = Vector2.left;
        SnakeSegments.Add(transform);
        AddNewSegment(snakeHead, transform.position);
        AddNewSegment(snakeBody, transform.position + new Vector3(1, 0, 0));
        AddNewSegment(snakeBody, transform.position + new Vector3(2, 0, 0));
        StartCoroutine(MoveSnake());
    }

    private void FixedUpdate()
    {
        var horizontalInput = Input.GetAxisRaw(HorizontalInput);
        var verticalInput = Input.GetAxisRaw(VerticalInput);

        // Controls the snake movimentation
        if (_canMove)
        {
            if (horizontalInput != 0 && verticalInput == 0 && _isMovingVertically)
            {
                _canMove = false;
                _direction = new Vector2(horizontalInput, 0);
                _isMovingVertically = false;
            }
            else if (verticalInput != 0 && horizontalInput == 0 && !_isMovingVertically)
            {
                _canMove = false;
                _direction = new Vector2(0, verticalInput);
                _isMovingVertically = true;
            }
        }
    }

    public void SetFoodTarget(Transform food)
    {
        _currentFoodType = food.GetComponent<SnakeFood>().foodType;
    }

    public void SpawnSnake()
    {
        _timeToMove = DefaultTimeToMove;
        _isMovingVertically = false;
        gameObject.transform.position = SpawnPosition;
        gameObject.SetActive(true);
    }

    public void DestroySnake()
    {
        for (var index = SnakeSegments.Count - 1; index > 0; index--)
        {
            Destroy(SnakeSegments[index].gameObject);
        }

        SnakeSegments.Clear();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(FoodTag))
        {
            Destroy(collider.gameObject);
            AddNewSegment(snakeBody, _lastPosition);
            _timeToMove += _currentFoodType == FoodType.SpeedFood ?
                SpecialFoodTime : NormalFoodTime;

            // Adds an extra life when eating the related food
            if (_currentFoodType == FoodType.LifeFood && _extraBlocks < 3)
            {
                LifeGained?.Invoke(_extraBlocks);
                _extraBlocks++;
            }

            FoodEaten?.Invoke();
        }
        else if (collider.CompareTag(SnakeTag))
        {
            if (_extraBlocks > 0)
            {
                _extraBlocks--;
                var lastSegment = SnakeSegments.Last();
                SnakeSegments.Remove(lastSegment);
                Destroy(lastSegment.gameObject);
                LifeLost?.Invoke(_extraBlocks);
            }
            else
            {
                KillSnake();
            }
        }
        else
        {
            KillSnake();
        }
    }

    private IEnumerator MoveSnake()
    {
        _lastPosition = SnakeSegments.Last().position;
        transform.position += _direction;

        for (var index = SnakeSegments.Count - 1; index > 0; index--)
        {
            SnakeSegments[index].position = SnakeSegments[index - 1].position;
        }

        yield return new WaitForSeconds(_timeToMove);
        _canMove = true;
        StartCoroutine(MoveSnake());
    }

    private void AddNewSegment(Transform snakeSegment, Vector3 position)
    {
        var newSegment = Instantiate(snakeSegment);
        SnakeSegments.Add(newSegment);
        newSegment.position = position;
    }

    private void KillSnake()
    {
        DestroySnake();
        _extraBlocks = 0;
        LifeLost.Invoke(_extraBlocks);
        SnakeDied?.Invoke();
    }

    public void AddFoodEatenListener(Action action)
    {
        FoodEaten = action;
    }

    public void AddSnakeDiedListener(Action action)
    {
        SnakeDied = action;
    }

    public void AddLifeGainedListener(Action<int> action)
    {
        LifeGained = action;
    }

    public void AddLifeLostListener(Action<int> action)
    {
        LifeLost = action;
    }
}
