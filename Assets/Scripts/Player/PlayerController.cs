using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    public bool tutorialDone = false;
    public bool isOnPaintZone = false;
    private float verticalRotation = 0f;
    [SerializeField] private ColorPicked colorPicked; // Referencia al script del color que agafem la sargantana

    [Header("PickupSargantana")]
    [SerializeField] float pickupRange;
    [SerializeField] LayerMask pickupLayerMask;
    [SerializeField] LayerMask scultureMask;
    private MeshRenderer sargantanaMeshRenderer;
    public bool sargantanaAgafada = false;
    public ParticleSystem particulesSargantana;
    [SerializeField] private Transform cameraFollowTarget;

    [SerializeField] Image PunteroImage;
    [SerializeField] Sprite puntero1;
    [SerializeField] Sprite puntero2;

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


        sargantanaMeshRenderer = GameObject.Find("SargantanaAgafada").GetComponent<MeshRenderer>();
        particulesSargantana = gameObject.GetComponentInChildren<ParticleSystem>();

        sargantanaMeshRenderer.enabled = false;
    }

    private void Update()
    {
        HandleMouseLook();

        if (frozen) return;

        HandleMovementInput();
        CheckGround();

<<<<<<< HEAD
        if (Mouse.current.leftButton.wasPressedThisFrame && tutorialDone) //si clica el boto esquerre
=======

        if (!sargantanaAgafada && Mouse.current.leftButton.wasPressedThisFrame)
>>>>>>> f32bcee7934d7e9aa015418e4642ec2baf041dd2
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit; //tira un raig

            if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask) && !sargantanaAgafada) //si el raig colisiona con un objeto dentro del rango y en la capa correcta
            {
                if (hit.collider.CompareTag("sargantana"))
                {
                    Debug.Log("Recojer");

                    Drac drac = hit.collider.gameObject.GetComponent<Drac>();

                    if (drac != null)
                    {
                        colorPicked.DragonPicked(drac);
                    }

                    Recollir(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("El objeto no es recogible: " + hit.collider.name);
                }
            }
            if (Physics.Raycast(ray, out hit, pickupRange, scultureMask) && sargantanaAgafada && isOnPaintZone)
            {
                if (hit.collider.CompareTag("sculture"))
                {
                    ChangeColor changeColor = hit.collider.gameObject.GetComponent<ChangeColor>();

                    if (changeColor != null)
                    {
                        changeColor.ChangeColorSculture(colorPicked.currentColor);
                        Soltar();
                    }
                }
            }
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame && !isOnPaintZone && tutorialDone) // Si el jugador hace clic izquierdo mientras no está en una zona de pintura, suelta la sargantana
        {
            Soltar();
        }
    }

    private void FixedUpdate()
    {
        if(frozen) return;
        // Aplicar movimiento en FixedUpdate para física coherente
        ApplyMovement();
        ApplyGravity();
        CheckFrontalObject();
    }

    private void CheckFrontalObject()
    {
        PunteroImage.sprite = puntero1;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
        {
            if (hit.collider.CompareTag("sargantana"))
            {
                PunteroImage.sprite = puntero2;
            }
        }
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
        cameraFollowTarget.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PaintZone"))
        {
            isOnPaintZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PaintZone"))
        {
            isOnPaintZone = false;
        }
    }

    private bool frozen = false;

    public void SetFrozen(bool value)
    {
        frozen = value;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = value; // Si está congelado, no aplicar física
        }
           
    }
}