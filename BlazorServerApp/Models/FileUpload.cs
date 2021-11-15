using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
namespace BlazorServerApp.Models
{
    public class FileUpload :ComponentBase
    {
        protected bool Error { get; set; }
        protected bool spinning { get; set; }
        protected int InsertPercent { get; set; }
        protected string Message { get; set; } = "No file(s) selected";

        protected IReadOnlyList<IBrowserFile> selectedFiles;
        public const int largestFileSize = 1873691000;

        protected bool isChecked { get; set; } = true;

        protected void Toggle(bool Checked)
        {
            isChecked = Checked;
        }

        protected void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFiles = e.GetMultipleFiles();
            Message = $"{selectedFiles.Count} file(s) selected";
            this.StateHasChanged();
        }

    }
}
