using UnityEngine;

[System.Serializable] //Comme [SerializField] mais pour une classe
public class ParallaxLayer  // Classe pour gérer les couches de parallax, avec des vitesses de déplacement différentes pour créer un effet de profondeur
{
    public float speedX = 0.5f;
    public float speedY = 0.2f;

    private Transform _transform;
    private Vector3 _targetPosition;


    private SpriteRenderer _sprite;
    private float _spriteWidth;
    private bool _infiniteX;

    // ---- scaling related
    private bool _scaleWithCamera = false;
    private float _minScale = 0.5f;
    private float _maxScale = 1.5f;
    private float _referenceOrthoSize = 5f;
    private float _scaleSmoothing = 0.1f;
    private Vector3 _initialScale;
    // ---------------------

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
            speedX = settings.speedX;
            speedY = settings.speedY;
            _scaleWithCamera = settings.scaleWithCamera;
            _minScale = settings.minScale;
            _maxScale = settings.maxScale;
            _referenceOrthoSize = settings.referenceOrthoSize;
            _scaleSmoothing = settings.scaleSmoothing;
            _initialScale = _transform.localScale;
        }
    }

    public void Move(Vector3 delta, bool vertical, float smoothing) // Parametres --> info dont la fonction a besoin pour fonctionner, ex ici a besoin d'un Vector3, un bool et un float stocker dans leurs variables associésS
    {
        float moveX = delta.x * (1f - speedX);
        float moveY = vertical ? delta.y * (1f - speedY) : 0f; // si vertical = true, alors on fait la suite, si false moveY = 0f
        //Faire en sorte que le sprite grosisse en même temps que la caméra s'éloigne, et rapetisse lorsque la caméra se rapproche
        

        _targetPosition += new Vector3(moveX, moveY, 0f);
        _transform.position = smoothing > 0f ? Vector3.Lerp(_transform.position, _targetPosition, smoothing) : _targetPosition;

        // Apply scaling based on camera ortho size if enabled
        if (_scaleWithCamera)
        {
            float camOrtho = Camera.main != null ? Camera.main.orthographicSize : _referenceOrthoSize;
            float factor = (_referenceOrthoSize > 0f) ? camOrtho / _referenceOrthoSize : 1f;
            factor = Mathf.Clamp(factor, _minScale, _maxScale);

            Vector3 targetScale = _initialScale * factor;
            _transform.localScale =  _scaleSmoothing > 0f ? Vector3.Lerp(_transform.localScale, targetScale, _scaleSmoothing) : targetScale;
        }

        if (_infiniteX)
        {
            WrapHorizontal();
        }
    }

    private void WrapHorizontal() // Fonction pour faire en sorte que les couches de parallaxe se répètent horizontalement lorsque la caméra se déplace
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


