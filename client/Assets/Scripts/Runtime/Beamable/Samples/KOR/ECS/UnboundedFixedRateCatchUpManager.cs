using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace Beamable.Samples.KOR.Multiplayer
{
/// <summary>
    /// The only difference between this class and the FixedRateCatchUpManager is that there is no upper bound to the max delta time.
    /// </summary>
    public class UnboundedFixedRateCatchUpManager : IFixedRateManager
    {
        // TODO: move this to World
        float m_MaximumDeltaTime;

        public float MaximumDeltaTime
        {
            get => m_MaximumDeltaTime;
            set => m_MaximumDeltaTime = math.max(value, m_FixedTimestep);
        }

        float m_FixedTimestep;

        public float Timestep
        {
            get => m_FixedTimestep;
            set { m_FixedTimestep = math.clamp(value, .0001f, 100000); }
        }

        double m_LastFixedUpdateTime;
        long m_FixedUpdateCount;
        bool m_DidPushTime;
        double m_MaxFinalElapsedTime;

        public UnboundedFixedRateCatchUpManager(float fixedDeltaTime)
        {
            Timestep = fixedDeltaTime;
        }

        public bool ShouldGroupUpdate(ComponentSystemGroup group)
        {
            float worldMaximumDeltaTime = group.World.MaximumDeltaTime;
            float maximumDeltaTime = math.max(worldMaximumDeltaTime, m_FixedTimestep);

            // if this is true, means we're being called a second or later time in a loop
            if (m_DidPushTime)
            {
                group.World.PopTime();
            }
            else
            {
                m_MaxFinalElapsedTime = m_LastFixedUpdateTime + maximumDeltaTime;
            }

            var finalElapsedTime = math.min(m_MaxFinalElapsedTime, group.World.Time.ElapsedTime);
            if (m_FixedUpdateCount == 0)
            {
                // First update should always occur at t=0
            }
            else if (finalElapsedTime - m_LastFixedUpdateTime >= m_FixedTimestep)
            {
                // Advance the timestep and update the system group
                m_LastFixedUpdateTime += m_FixedTimestep;
            }
            else
            {
                // No update is necessary at this time.
                m_DidPushTime = false;
                return false;
            }

            m_FixedUpdateCount++;

            group.World.PushTime(new TimeData(
                elapsedTime: m_LastFixedUpdateTime,
                deltaTime: m_FixedTimestep));

            m_DidPushTime = true;
            return true;
        }
    }

}