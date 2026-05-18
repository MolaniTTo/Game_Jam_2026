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
    public bool isOnMenuMode = false;

    [Header("PickupSargantana")]
    [SerializeField] float pickupRange;
    [SerializeField] LayerMask pickupLayerMask;
    [SerializeField] LayerMask scultureMask;
    private MeshRenderer sargantanaMeshRenderer;
    public bool sargantanaAgafada = false;
    public ParticleSystem particulesSargantana;
    public ParticleSystem particulesChorro;
    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private Transform sargantanaSpawn;
    [SerializeField] private GameObject sargantanaAgafadaPrefab;
    private GameObject sargantanaAgafadaInstance = null;
    [SerializeField] private Manotazo manotazo;

    [SerializeField] Image PunteroImage;
    [SerializeField] Sprite puntero1;
    [SerializeField] Sprite puntero2;

    [SerializeField] MeshRenderer meshMaTancada;
    [SerializeField] ParticleSystem particulesImpacteCorrecte;


    [Header("SphereCast")]
    [SerializeField] private float sphereRadius = 0.3f;

    [Header("Sons")]
    private AudioSource audioSource;
    [SerializeField] AudioClip stepSound;
    [SerializeField] AudioClip handSound;
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip paintSound;

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

        meshMaTancada.enabled = false;

        audioSource = gameObject.GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (isOnMenuMode) return;

        HandleMouseLook();

        if (frozen) return;

        HandleMovementInput();
        CheckGround();

        if (Mouse.current.leftButton.wasPressedThisFrame && tutorialDone)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (sargantanaAgafada)
            {
                // PRIORIDAD 1: Si tenemos sargantana, intentar pintar escultura
                // Raycast preciso y más largo que el pickup
                float paintRange = pickupRange * 2.5f;
                if (Physics.Raycast(ray.origin, ray.direction, out hit, paintRange, scultureMask))
                {
                    if (hit.collider.CompareTag("sculture"))
                    {
                        WhatColorAmI whatColor = hit.collider.gameObject.GetComponent<WhatColorAmI>();
                        if (whatColor != null)
                        {
                            bool eraPintada = whatColor.IsPainted;
                            whatColor.TryPaint(colorPicked.colorSO);
                            audioSource.PlayOneShot(paintSound);

                            if (whatColor.IsPainted && !eraPintada)
                            {
                                ParticleSystem ps = Instantiate(particulesImpacteCorrecte, hit.point, Quaternion.LookRotation(hit.normal));
                                var main = ps.main;
                                main.startColor = whatColor.GetValidColor().color;
                                ps.Play();
                                Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                            }

                            Soltar();
                        }
                    }
                }
                // Si tenemos sargantana pero no apuntamos a escultura: no pasa nada
            }
            else
            {
                // PRIORIDAD 2: Sin sargantana, intentar recoger con SphereCast
                if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out hit, pickupRange, pickupLayerMask))
                {
                    if (hit.collider.CompareTag("sargantana"))
                    {
                        Drac drac = hit.collider.gameObject.GetComponent<Drac>();
                        if (drac != null)
                            colorPicked.DragonPicked(drac);
                        Recollir(hit.collider.gameObject);
                        return; // Salimos para no ejecutar el manotazo
                    }
                }

                // PRIORIDAD 3: Manotazo si no hay nada que recoger
                manotazo.Ejecutar();
                audioSource.PlayOneShot(handSound);
            }
        }

        // Click derecho: soltar en cualquier situación
        if (Mouse.current.rightButton.wasPressedThisFrame && tutorialDone && sargantanaAgafada)
        {
            Soltar();
        }
    }

    private void FixedUpdate()
    {
        if (frozen) return;
        // Aplicar movimiento en FixedUpdate para física coherente
        ApplyMovement();
        ApplyGravity();
        CheckFrontalObject();
    }

    private void CheckFrontalObject()
    {
        PunteroImage.sprite = puntero1;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayerMask))
        {
            if (hit.collider.CompareTag("sargantana") && !sargantanaAgafada)
            {
                PunteroImage.sprite = puntero2;
            }
        }
    }

    private void Recollir(GameObject sargantanaGameObject)
    {
        Destroy(sargantanaGameObject);
        sargantanaAgafadaInstance = Instantiate(
            sargantanaAgafadaPrefab,
            sargantanaSpawn
        );
        sargantanaAgafadaInstance.GetComponent<ChangeColor>().ChangeColorSargantana(colorPicked.currentColor);
        sargantanaAgafadaInstance.transform.localPosition = Vector3.zero;
        sargantanaAgafadaInstance.transform.localRotation = Quaternion.identity;
        sargantanaAgafada = true;

        meshMaTancada.enabled = true;

        audioSource.PlayOneShot(pickUpSound);

    }

    private void Soltar()
    {
        if (sargantanaAgafadaInstance != null)
            Destroy(sargantanaAgafadaInstance);

        sargantanaAgafada = false;

        // Força alpha 1 per les partícules
        Color colorParticules = colorPicked.currentColor;
        colorParticules.a = 1f;

        var main1 = particulesSargantana.main;
        main1.startColor = colorParticules;
        particulesSargantana.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particulesSargantana.Play();

        var main2 = particulesChorro.main;
        main2.startColor = colorParticules;
        particulesChorro.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particulesChorro.Play();

        meshMaTancada.enabled = false;
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

        // // SONIDO PASOS
        // bool isMoving = inputDirection.magnitude > 0.1f;

        // if (isMoving && isGrounded)
        // {
        //     if (!audioSource.isPlaying)
        //     {
        //         audioSource.clip = stepSound;
        //         audioSource.loop = true;
        //         audioSource.Play();
        //     }

        //     audioSource.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.4f : 1f;
        // }
        // else
        // {
        //     if (audioSource.isPlaying)
        //     {
        //         audioSource.Stop();
        //     }
        // }
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

    // Deixar comentat
    // private void UpdateCursorLock()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         Cursor.lockState = CursorLockMode.None;
    //         Cursor.visible = true;
    //     }
    //     else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
    //     {
    //         Cursor.lockState = CursorLockMode.Locked;
    //         Cursor.visible = false;
    //     }
    // }

    // private void LateUpdate()
    // {
    //     UpdateCursorLock();
    // }

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