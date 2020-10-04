using System.IO;
using System.Windows.Forms;

namespace LSSDReportHelper.Engines
{
    public class PatrolEngine
    {
        private readonly Timer _patrolTimer = new Timer();
        private readonly ScreenShooter _screenShooter = new ScreenShooter();
        private string _path;
        private string _pathPrifex;
        private bool _patrolState;

        public PatrolEngine(uint interval = 10, string basePath = "d:\\")
        {
            _path = basePath;
            _patrolTimer.Interval = (int)interval * 1000 * 60;
            _patrolTimer.Tick += (o, e) => _screenShooter.SaveImage(_path + "\\" + _pathPrifex);
        }

        public void StartPatrol()
        {
            if (_patrolState)
            {
                _screenShooter.SaveImage(_path + "\\" + _pathPrifex);
                _patrolTimer.Stop();
                _patrolState = false;
            }
            else
            {
                GetPathPrifex();
                _screenShooter.SaveImage(_path + "\\" + _pathPrifex);
                _patrolTimer.Start();
                _patrolState = true;
            }
        }

        public void ChangeInterval(uint newInterval)
        {
            if (!_patrolState) 
                _patrolTimer.Interval = (int)newInterval * 1000 * 60;
        }

        public void ChangeFolder(string newPath)
        {
            if (!_patrolState) _path = newPath;
        }

        private void GetPathPrifex()
        {
            var dirCount = Directory.GetDirectories(_path).Length;
            Directory.CreateDirectory(_path + "\\" + $"Патруль {dirCount + 1}");
            _pathPrifex = $"Патруль {dirCount + 1}";
        }
    }
}
