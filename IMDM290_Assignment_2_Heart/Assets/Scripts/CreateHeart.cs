using UnityEngine;

public class CreateHeart : MonoBehaviour
{

    public float size;
    public int numSpheres;
    public float colorWaveSpeed;
    public Material sphereMaterial;

    private GameObject[] spheres;
    private float root2 = Mathf.Sqrt(2);
    private float hueOffset = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spheres = new GameObject[numSpheres];
        for (int i = 0; i < numSpheres; i++)
        {
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            float theta = i * 2 * Mathf.PI / numSpheres;

            // sqrt(2) * sin^3(theta)
            float xPos = root2 * Mathf.Pow(Mathf.Sin(theta), 3);

            // -cos^3(theta) - cos^2(theta) + 2cos(theta)
            float yPos = -Mathf.Pow(Mathf.Cos(theta), 3) - Mathf.Cos(theta) * Mathf.Cos(theta) + 2 * Mathf.Cos(theta);

            spheres[i].transform.parent = transform;
            spheres[i].transform.localPosition = new Vector3(size * xPos, size * yPos, i * 0.0001f);
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            sphereRenderer.material = sphereMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float hueChangePerSphere = 1f / numSpheres;
        for (int i = 0; i < numSpheres; i++)
        {
            // Set Color
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            float hue;

            if (i % 4 == 0)
            {
                hue = (hueOffset + i * hueChangePerSphere) % 1;
            }
            else
            {
                hue = (hueOffset + (numSpheres - i) * hueChangePerSphere) % 1;
            }

            Color color = Color.HSVToRGB(hue, 1f, 1f);

            sphereRenderer.material.color = color;

            // Set Position
            Vector3 curSpherePos = spheres[i].transform.localPosition;
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 15f; // Distance between camera and spheres

            //float distance = Vector3.Distance(curSpherePos, Camera.main.ScreenToWorldPoint(mousePosition));
            float theta = i * 2 * Mathf.PI / numSpheres;

            // sqrt(2) * sin^3(theta)
            float xPos = root2 * Mathf.Pow(Mathf.Sin(theta), 3);

            // -cos^3(theta) - cos^2(theta) + 2cos(theta)
            float yPos = -Mathf.Pow(Mathf.Cos(theta), 3) - Mathf.Cos(theta) * Mathf.Cos(theta) + 2 * Mathf.Cos(theta);

            Vector3 baseSpherePosition = new Vector3(size * xPos, size * yPos, i * 0.0001f);
            float distance = Vector3.Distance(baseSpherePosition, Camera.main.ScreenToWorldPoint(mousePosition));

            float inverseDistance = Mathf.Max(1f - distance / 3, 0f) * 3f;

            Vector3 direction = Vector3.Normalize(new Vector2(xPos, yPos));

            if (direction.x == 0 && direction.y == 0)
            {
                direction.y = 1f;
            }

            System.Random random = new System.Random(i);

            direction.x += (float) random.NextDouble() * 2 - 1;
            direction.y += (float) random.NextDouble() * 2 - 1;

            direction *= inverseDistance;

            spheres[i].transform.localPosition = new Vector3(size * xPos + direction.x, size * yPos + direction.y, i * 0.0001f);
        }

        // Move Heart Itself
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 15f; // Distance between camera and spheres
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.rotation = Quaternion.Euler(worldMousePos.y, -worldMousePos.x, 0);

        // Increase hueOffset for next frame
        hueOffset += colorWaveSpeed;
    }
}