using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0; // Todo excepto capas ignoradas

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalLookLimit = 80f;

    [Header("References")]
    [SerializeField] private Camera playerCamera; // Asignar la cámara desde el inspector
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float verticalRotation = 0f;

    [Header("PickupSargantana")]
    [SerializeField] float pickupRange;
    [SerializeField] LayerMask pickupLayerMask;
    private MeshRenderer sargantanaMeshRenderer;
    public bool sargantanaAgafada = false;
    public ParticleSystem particulesSargantana;

    private void Start()
    {
        // Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Obtener Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        // Configurar Rigidbody para un FPS
        rb.freezeRotation = true; // Evita que el Rigidbody gire por físicas
        rb.mass = 1f;
        rb.linearDamping = 0f;

        // Si no se asignó cámara, buscar una en los hijos
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        sargantanaMeshRenderer = GameObject.Find("SargantanaAgafada").GetComponent<MeshRenderer>();
        particulesSargantana = gameObject.GetComponentInChildren<ParticleSystem>();

        sargantanaMeshRenderer.enabled = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovementInput();
        CheckGround();

        if (!sargantanaAgafada && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
            {
                if (hit.collider.CompareTag("sargantana"))
                {
                    Debug.Log("Recojer");
                    Recollir(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("El objeto no es recogible: " + hit.collider.name);
                }
            }
        } 
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Soltar();
        }
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento en FixedUpdate para física coherente
        ApplyMovement();
        ApplyGravity();
    }

    private void Recollir(GameObject sargantanaGameObject)
    {
        Destroy(sargantanaGameObject);
        sargantanaMeshRenderer.enabled = true;
        sargantanaAgafada = true;
    }

    private void Soltar()
    {
        sargantanaAgafada = false;
        sargantanaMeshRenderer.enabled = false;
        particulesSargantana.Play();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal del cuerpo (eje Y)
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical de la cámara (eje X local)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Dirección relativa a la orientación del jugador
        Vector3 inputDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        moveDirection = inputDirection * currentSpeed;
    }

    private void ApplyMovement()
    {
        // Movimiento horizontal: mantener velocidad horizontal actual
        Vector3 targetVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        rb.linearVelocity = targetVelocity;
    }

    private void CheckGround()
    {
        // Lanzar un rayo desde los pies del jugador hacia abajo
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Ligero offset para evitar auto-colisiones
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance + 0.1f, groundMask);
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Aplicar gravedad si no está en el suelo
            rb.linearVelocity += Vector3.up * gravity * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y < 0)
        {
            // Resetear velocidad vertical si está en el suelo y cayendo
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }

    // Opcional: liberar cursor con Escape
    private void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        UpdateCursorLock();
    }
}