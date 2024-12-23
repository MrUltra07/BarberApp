using BarberApp.Models;
using BarberApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public class GeneralSettingsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public GeneralSettingsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet("generalsettings")]
    public IActionResult Index()
    {
        var settings = _context.GeneralSettings.FirstOrDefault(g => g.Id == 1);
        if (settings == null) return NotFound();

        var viewModel = new GeneralSettingsViewModel
        {
            Id = settings.Id,
            Name = settings.Name,
            Description = settings.Description,
            LogoUrl = settings.LogoUrl,
            SeoTitle = settings.SeoTitle,
            SeoDescription = settings.SeoDescription,
            Keywords = settings.Keywords
        };

        return View("~/Views/Admin/GeneralSettings/Index.cshtml", viewModel);
    }

    [HttpPost("generalsettings/update")]
    public IActionResult Update(GeneralSettingsViewModel viewModel)
    {
        // Her zaman ID = 1 olan kaydı alıyoruz
        var settings = _context.GeneralSettings.FirstOrDefault(g => g.Id == 1);
        if (settings == null) return NotFound();

        // Verileri güncelle
        settings.Name = viewModel.Name;
        settings.Description = viewModel.Description;
        settings.SeoTitle = viewModel.SeoTitle;
        settings.SeoDescription = viewModel.SeoDescription;
        settings.Keywords = viewModel.Keywords;

        // Dosya işlemleri
        if (viewModel.LogoFile != null)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(viewModel.LogoFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                viewModel.LogoFile.CopyTo(stream);
            }

            // Eski logoyu sil
            if (!string.IsNullOrEmpty(settings.LogoUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, settings.LogoUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            // Yeni logo URL'sini kaydet
            settings.LogoUrl = "/uploads/" + fileName;
        }

        // Değişiklikleri kaydet
        _context.SaveChanges();
        TempData["Status"] = "ok";
        TempData["Message"] = "General settings updated successfully!";
        return RedirectToAction("Index");
    }


    [HttpGet("generalsettings/update")]
    public IActionResult Update()
    {
        return RedirectToAction("Index");
    }
}
