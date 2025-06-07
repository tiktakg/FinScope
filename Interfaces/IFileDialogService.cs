using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.Interfaces
{
    public interface IFileDialogService
    {
        Task<string?> OpenImageFileAsync();
    }
}
