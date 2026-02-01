using System.Numerics;

namespace LightOnSpotCore.DMX.Fixtures
{
    public class LightingDroneData
    {
        private LightingDrone drone;
        public LightingDrone Drone { get { return drone; } }

        private Vector3 startPosition;
        public Vector3 StartPosition { get { return startPosition; } set { startPosition = value; } }

        private Vector3 currentPosition;
        public Vector3 CurrentPosition { get { return currentPosition; } set { currentPosition = value; }  }

        public LightingDroneData(LightingDrone lightingDrone)
        {
            this.drone = lightingDrone;
        }
    }
}
