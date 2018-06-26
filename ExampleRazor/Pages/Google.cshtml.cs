using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace RazorExamples.Pages
{
    public class GoogleModel : PageModel
    {
        public readonly IConfiguration Configuration;

        public GoogleModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnGet()
        {

        }
    }
}
