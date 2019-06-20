using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetMark
{
    /// <summary>
    /// Provides wait cursor function
    /// </summary>
    /// <remarks>
    /// See https://stackoverflow.com/questions/3480966/display-hourglass-when-application-is-busy?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
    /// </remarks>
    internal class WaitCursor : IDisposable
    {
        private static readonly object LockObject = new object();

        private static int _mCounter = 0;
        private static Cursor _mPreviousCursor;

        public WaitCursor()
        {
            lock (LockObject)
            {
                if (_mCounter == 0)
                {
                    _mPreviousCursor = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                }
                _mCounter++;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (LockObject)
            {
                _mCounter--;
                if (_mCounter == 0)
                {
                    Mouse.OverrideCursor = _mPreviousCursor;
                }
            }
        }

        #endregion
    }
}
