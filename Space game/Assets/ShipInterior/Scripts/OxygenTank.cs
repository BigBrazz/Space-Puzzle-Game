using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    [SerializeField] private float oxygenAmount = 20f;
    [SerializeField] private Transform visual;
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float pickupVolume = 1f;

    private OxygenSystem oxygenSystem;
    private Vector3 startPosition;
    private float bobOffset;

    private void Start()
    {
        if (visual == null)
            visual = transform;

        startPosition = visual.localPosition;
        bobOffset = Random.Range(0f, 6.28f);
        oxygenSystem = FindObjectOfType<OxygenSystem>();
    }

    private void Update()
    {
        if (visual == transform)
            return;

        visual.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight;
        visual.localPosition = pos;
    }

    // OnTriggerStay too, so you can come back for a tank you skipped at full oxygen
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || oxygenSystem == null)
            return;

        if (oxygenSystem.AddOxygen(oxygenAmount))
        {
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

            Destroy(gameObject);
        }
    }
}
