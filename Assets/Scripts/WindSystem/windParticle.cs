using UnityEngine;

[RequireComponent(typeof(AreaEffector2D))]
public class WindParticle : MonoBehaviour
{
    private AreaEffector2D effector;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    public ParticleSystem.NoiseModule noiseModule;

    [Header("速度倍率")]
    public float force = 0.25f;

    void Awake()
    {
        effector = GetComponent<AreaEffector2D>();
        ps = GetComponentInChildren<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        noiseModule = ps.noise;
    }

    private Vector2 GetWindVector()
    {
        float angleRad = effector.forceAngle * Mathf.Deg2Rad;
        float mag = effector.forceMagnitude * 0.01f;
        Vector2 dir;
        if (effector.useGlobalAngle)
        {
            dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        else
        {
            Quaternion rot = transform.rotation;
            dir = rot * new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        return dir * mag;
    }

    void LateUpdate()
    {
        if (!effector.enabled)
        {
            ps.Stop();
            return;
        }
        if (!ps.isPlaying) ps.Play();

        Vector2 wind = GetWindVector();
        float windMag = wind.magnitude;
        if (windMag < 0.01f) return;

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            Vector3 vel = (Vector3)wind.normalized * windMag * force;
            particles[i].velocity = vel;
        }
        ps.SetParticles(particles, count);
    }
}