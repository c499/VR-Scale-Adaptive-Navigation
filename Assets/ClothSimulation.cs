using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothSimulation : MonoBehaviour
{
    // Struct to hold information about each particle in the cloth
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 force;
        public float mass;
    }

    // Array of particles comprising the cloth
    Particle[] particles;

    // Spring constants for structural and shear springs
    public float kStructural = 30.0f;
    public float kShear = 20.0f;

    // Spring constants for flexion springs
    public float kFlexion = 15.0f;

    // Damping coefficient
    public float damping = 5.0f;

    // Time step for simulation
    public float deltaTime = 0.01f;

    // Number of particles in each dimension of the cloth grid
    public int numParticlesX = 10;
    public int numParticlesY = 10;

    // Spacing between particles
    public float particleSpacing = 0.2f;

    // Gravity acceleration
    public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

    void Start()
    {
        // Initialize the particles array
        particles = new Particle[numParticlesX * numParticlesY];

        // Initialize the positions and masses of the particles
        for (int y = 0; y < numParticlesY; y++)
        {
            for (int x = 0; x < numParticlesX; x++)
            {
                int index = x + y * numParticlesX;
                particles[index].position = new Vector3(x * particleSpacing, y * particleSpacing, 0.0f);
                particles[index].mass = 1.0f;
            }
        }
    }

    void Update()
    {
        // Apply forces to the particles
        ApplyForces();

        // Integrate the particle positions and velocities
        Integrate();
    }

    void ApplyForces()
    {
        // Clear the forces on all particles
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].force = Vector3.zero;
        }

        // Add gravity force to all particles
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].force += gravity * particles[i].mass;
        }

        // Add spring forces to the particles
        AddSpringForces();
    }
    void AddSpringForces()
    {
        // Structural and shear spring forces
        for (int y = 0; y < numParticlesY; y++)
        {
            for (int x = 0; x < numParticlesX; x++)
            {
                int index = x + y * numParticlesX;

                // Structural spring forces to the left and above
                if (x > 0)
                {
                    AddSpringForce(index, index - 1, kStructural);
                }
                if (y > 0)
                {
                    AddSpringForce(index, index - numParticlesX, kStructural);
                }

                // Shear spring forces to the upper left and upper right
                if (x > 0 && y > 0)
                {
                    AddSpringForce(index, index - numParticlesX - 1, kShear);
                    AddSpringForce(index, index - numParticlesX + 1, kShear);
                }

                // Flexion spring forces to the lower left and lower right
                if (x > 0 && y < numParticlesY - 1)
                {
                    AddSpringForce(index, index + numParticlesX - 1, kFlexion);
                    AddSpringForce(index, index + numParticlesX + 1, kFlexion);
                }
            }
        }
    }
    void AddSpringForce(int index1, int index2, float springConstant)
    {
        Particle p1 = particles[index1];
        Particle p2 = particles[index2];

        Vector3 displacement = p1.position - p2.position;
        float distance = displacement.magnitude;
        Vector3 direction = displacement / distance;

        float springForce = -springConstant * (distance - particleSpacing);
        p1.force += springForce * direction;
        p2.force -= springForce * direction;

        particles[index1] = p1;
        particles[index2] = p2;
    }
    void Integrate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Particle p = particles[i];

            // Update the velocity and position of the particle using the Euler method
            p.velocity += p.acceleration * deltaTime;
            p.position += p.velocity * deltaTime;

            // Apply damping to the velocity
            p.velocity *= 1.0f - damping * deltaTime;

            // Calculate the new acceleration of the particle
            p.acceleration = p.force / p.mass;

            particles[i] = p;
        }
    }
}