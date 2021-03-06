using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySnakeBehavior : MonoBehaviour
{
    private readonly List<Transform> SnakeSegments = new List<Transform>();
    private readonly Vector3 SpawnPosition = new Vector3(-20, -10, 0);

    private const string PlayerTag = "Player";
    private const string FoodTag = "Food";

    private const float NormalFoodTime = 0.02f;
    private const float SpecialFoodTime = -0.04f;
    private const float DefaultTimeToMove = 0.2f;

    [SerializeField] private Transform snakeHead;
    [SerializeField] private Transform snakeBody;

    private Transform _spawnedFood;
    private FoodType _currentFoodType;
    private Vector3 _direction;
    private Vector3 _lastPosition;
    private float _timeToMove = DefaultTimeToMove;

    private Action FoodEaten;

    private void OnEnable()
    {
        SnakeSegments.Add(transform);
        AddNewSegment(snakeHead, transform.position);
        AddNewSegment(snakeBody, transform.position - new Vector3(1, 0, 0));
        AddNewSegment(snakeBody, transform.position - new Vector3(2, 0, 0));
        StartCoroutine(MoveSnake());
    }

    private void FixedUpdate()
    {
        if (_spawnedFood)
        {
            var foodPosition = _spawnedFood.position;
            if (transform.position.y < foodPosition.y) _direction = Vector2.up;
            else if (transform.position.y > foodPosition.y) _direction = Vector2.down;
            else if (transform.position.x < foodPosition.x) _direction = Vector2.right;
            else if (transform.position.x > foodPosition.x) _direction = Vector2.left;
        }
        else
        {
            _direction = Vector2.right;
        }
    }

    public void SetFoodTarget(Transform food)
    {
        _spawnedFood = food;
        _currentFoodType = food.GetComponent<SnakeFood>().foodType;
    }

    public void SpawnSnake()
    {
        _timeToMove = DefaultTimeToMove;
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
            FoodEaten?.Invoke();
        }
        else if (!collider.CompareTag(PlayerTag))
        {
            DestroySnake();
            SpawnSnake();
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
        StartCoroutine(MoveSnake());
    }

    private void AddNewSegment(Transform snakeSegment, Vector3 position)
    {
        var newSegment = Instantiate(snakeSegment);
        SnakeSegments.Add(newSegment);
        newSegment.position = position;
    }

    public void AddFoodEatenListener(Action action)
    {
        FoodEaten = action;
    }
}
