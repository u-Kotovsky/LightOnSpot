namespace LightOnSpotLibrary
{
    public class ArtNetManager
    {
        private ControllerWrapper? controller;
        private NodeInputWrapper? nodeInput;
        private NodeOutputWrapper? nodeOutput;

        public void Start()
        {
            //StartController();
            StartNodeInput();
        }

        private void StartController()
        {
            controller = new ControllerWrapper();
            controller.Start();
        }

        private void StartNodeInput()
        {
            nodeInput = new NodeInputWrapper();
            nodeInput.Start();
        }

        private void StartNodeOutput()
        {
            nodeOutput = new NodeOutputWrapper();
            nodeOutput.Start();
        }
    }
}
