using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.Components.Forms;

namespace RecipeProcessorService
{
    public class RecipeProcessorService
    {
        private Timer _timer;
        private CircularQueue<string> _recipesToProcess = new CircularQueue<string>(500);

        public void QueueBrowserFilesForProcessing(List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> list)
        {

        }

    }
}
