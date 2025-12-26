using System;
using System.Collections.Generic;
using System.Text;

namespace LightOnSpotApp.Services
{
    public class ProjectService
    {
        private bool _projectLocked = false;
        public bool ProjectLocked { get { return _projectLocked; } }

        private static ProjectService instance;
        public static ProjectService Instance { get { return instance; } }

        public void CreateNewProject()
        {
            if (_projectLocked)
            {
                throw new Exception("Project is locked. Unlock first.");
            }

            _projectLocked = true;
        }

        public void OpenProject(string pathToProjFile)
        {
            if (_projectLocked)
            {
                throw new Exception("Project is locked. Unlock first.");
            }

            _projectLocked = true;
        }

        public void CloseProject()
        {
            if (!_projectLocked)
            {
                throw new Exception("Project was not locked.");
            }


            _projectLocked = true;
        }

    }
}
