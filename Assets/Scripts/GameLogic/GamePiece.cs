using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer pieceRenderer;
    [SerializeField] private Animator pieceAnimator;
    
    [Header("Visual Settings")]
    [SerializeField] private Material xMaterial;
    [SerializeField] private Material oMaterial;
    [SerializeField] private float placementAnimationDuration = 0.5f;
    
    private PlayerType _owner;
    private GridPosition _position;
    private int _moveNumber;
    
    public PlayerType Owner
    {
        get { return _owner; }
        set
        {
            _owner = value;
            UpdateVisuals();
        }
    }
    
    public GridPosition Position
    {
        get { return _position; }
        set { _position = value; }
    }
    
    public int MoveNumber
    {
        get { return _moveNumber; }
        set { _moveNumber = value; }
    }
    
    private void Awake()
    {
        if (pieceRenderer == null)
        {
            pieceRenderer = GetComponent<Renderer>();
        }
        
        if (pieceAnimator == null)
        {
            pieceAnimator = GetComponent<Animator>();
        }
    }
    
    private void UpdateVisuals()
    {
        if (pieceRenderer != null)
        {
            pieceRenderer.material = _owner == PlayerType.X ? xMaterial : oMaterial;
        }
    }
    
    public void PlayPlacementAnimation()
    {
        if (pieceAnimator != null)
        {
            pieceAnimator.SetTrigger("Place");
        }
        else
        {
            StartCoroutine(ScaleAnimation());
        }
    }
    
    private System.Collections.IEnumerator ScaleAnimation()
    {
        transform.localScale = Vector3.zero;
        
        float elapsedTime = 0f;
        while (elapsedTime < placementAnimationDuration)
        {
            float t = elapsedTime / placementAnimationDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = Vector3.one;
    }
    
    public void Highlight(bool isHighlighted)
    {
        if (pieceRenderer != null)
        {
            if (isHighlighted)
            {
                pieceRenderer.material.EnableKeyword("_EMISSION");
                pieceRenderer.material.SetColor("_EmissionColor", Color.yellow);
            }
            else
            {
                pieceRenderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
    
    public void Initialize(PlayerType playerType)
    {
        _owner = playerType;
        UpdateVisuals();
        
        _position = new GridPosition();
        _moveNumber = 0;
    }
}