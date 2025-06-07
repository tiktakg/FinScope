using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinScope.Interfaces;

namespace FinScope.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly Window _mainWindow;

        public FileDialogService(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public async Task<string?> OpenImageFileAsync()
        {
            var files = await _mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выберите фотографию",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            return files.FirstOrDefault()?.Path.LocalPath;
        }
    }
}
