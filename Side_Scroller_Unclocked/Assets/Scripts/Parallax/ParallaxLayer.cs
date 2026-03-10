using UnityEngine;

[System.Serializable] //Comme [SerializField] mais pour une classe
public class ParallaxLayer 
{
    public float speedX = 0.5f;
    public float speedY = 0.2f;

    private Transform _transform;
    private Vector3 _targetPosition;


    private SpriteRenderer _sprite;
    private float _spriteWidth;
    private bool _infiniteX;

    public ParallaxLayer(Transform t)
    {
        _transform = t;
        _targetPosition = t.position;
        _sprite = t.GetComponent<SpriteRenderer>();

        if (_sprite != null )
        {
            _spriteWidth = _sprite.bounds.size.x;
            //infiniteX = _spriteWidth >0f;
        }

        var settings = t.GetComponent<ParallaxLayerSettings>();
        if (settings != null)
        {
            Debug.Log("On assigne settings");
            speedX = settings.speedX;
            speedY = settings.speedY;
        }
    }

    public void Move(Vector3 delta, bool vertical, float smoothing) // Parametres --> info dont la fonction a besoin pour fonctionner, ex ici a besoin d'un Vector3, un bool et un float stocker dans leurs variables associésS
    {
        float moveX = delta.x * (1f - speedX);
        float moveY = vertical ? delta.y * (1f - speedY) : 0f; // si vertical = true, alors on fait la suite, si false moveY = 0f

        _targetPosition += new Vector3(moveX, moveY, 0f);
        _transform.position = smoothing > 0f ? Vector3.Lerp(_transform.position, _targetPosition, smoothing) : _targetPosition;

        if (_infiniteX)
        {
            WrapHorizontal();
        }
    }

    private void WrapHorizontal()
    {
        float camX = Camera.main.transform.position.x;
        float diffX = camX - _transform.position.x;

        if (Mathf.Abs(diffX) >= _spriteWidth)
        {
            float offset = diffX > 0f ? _spriteWidth : -_spriteWidth;

            _transform.position += new Vector3(offset, 0f, 0f);
        }
    }
}


