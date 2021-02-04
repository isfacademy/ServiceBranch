using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceBranch.Models;
using ServiceBranch.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Substitute = ServiceBranch.Models.Substitute;

namespace ServiceBranch.Controllers
{
    [Authorize]
    public class SubstitutesController : Controller
    {
        private readonly SubstitutionContext _context;
        private readonly IHostingEnvironment _env;
        private readonly string[] _extensions = new string[] { ".xls", ".xlsx" };



        public SubstitutesController(SubstitutionContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Substitutes
        [HttpGet]
        public IActionResult Index()
        {

            DateTime CurrentDate = DateTime.Now;
            SubstitueVM SVM = new SubstitueVM();

            ViewBag.StartYear = GlobalController.GenerateYearsDDL(CurrentDate.Year);
            ViewBag.StartMonth = GlobalController.GenerateMonthsDDL(CurrentDate.Month);

            ViewBag.EndYear = GlobalController.GenerateYearsDDL(CurrentDate.Year);
            ViewBag.EndMonth = GlobalController.GenerateMonthsDDL(CurrentDate.Month);
            ViewBag.Category = new SelectList(_context.Categories.OrderByDescending(c => c.Id), "Id", "Name");

            SVM.StartYear = CurrentDate.Year;
            SVM.StartMonth = CurrentDate.Month;
            SVM.EndYear = CurrentDate.Year;
            SVM.EndMonth = CurrentDate.Month;
            SVM.CategoryId = 5;
            SVM.Substitutes = _context.Substitutes.Where(m => m.SubstituteDate.Month == CurrentDate.Month).OrderBy(m => m.SubstituteDate).Include(s => s.Category).ToList();
            return View(SVM);
        }

        [HttpPost]

        public IActionResult Index(SubstitueVM SVM)
        {
            DateTime CurrentDate = DateTime.Now;

            var DaysInmonth = DateTime.DaysInMonth(SVM.EndYear, SVM.EndMonth);
            DateTime StartDate = new DateTime(SVM.StartYear, SVM.StartMonth, 1);
            DateTime EndDate = new DateTime(SVM.EndYear, SVM.EndMonth, DaysInmonth);

            if (StartDate < EndDate)
            {
                if (SVM.CategoryId == 5)
                {
                    SVM.Substitutes = _context.Substitutes.Where(m => m.SubstituteDate >= StartDate && m.SubstituteDate <= EndDate).OrderBy(c => c.SubstituteDate).Include(s => s.Category).ToList();
                }
                else
                {
                    SVM.Substitutes = _context.Substitutes.Where(m => m.SubstituteDate >= StartDate && m.SubstituteDate <= EndDate && m.CategoryId == SVM.CategoryId).OrderBy(c => c.SubstituteDate).Include(s => s.Category).ToList();
                }
            }
            else
            {
                if (SVM.CategoryId == 5)
                {
                    SVM.Substitutes = _context.Substitutes.Where(m => m.SubstituteDate.Month == CurrentDate.Month).OrderBy(c => c.SubstituteDate).Include(s => s.Category).ToList();
                }
                else
                {
                    SVM.Substitutes = _context.Substitutes.Where(m => m.SubstituteDate.Month == CurrentDate.Month && m.CategoryId == SVM.CategoryId).OrderBy(c => c.SubstituteDate).Include(s => s.Category).ToList();
                }
                SVM.StartYear = CurrentDate.Year;
                SVM.StartMonth = CurrentDate.Month;
                SVM.EndYear = CurrentDate.Year;
                SVM.EndMonth = CurrentDate.Month;
            }

            ViewBag.StartYear = GlobalController.GenerateYearsDDL(SVM.StartYear);
            ViewBag.StartMonth = GlobalController.GenerateMonthsDDL(SVM.StartMonth);
            ViewBag.EndYear = GlobalController.GenerateYearsDDL(SVM.EndYear);
            ViewBag.EndMonth = GlobalController.GenerateMonthsDDL(SVM.EndMonth);
            ViewBag.Category = new SelectList(_context.Categories.OrderByDescending(c => c.Id), "Id", "Name", SVM.CategoryId);
            return View(SVM);
        }


        // GET: Substitutes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var substitute = await _context.Substitutes
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (substitute == null)
            {
                return NotFound();
            }

            return View(substitute);
        }

        // GET: Substitutes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        public IActionResult UploadExcel()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // import excel 
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IFormFile importFile)
        {
            string folderName = "Media\\ExcelUploaded";
            string webRootPath = _env.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            
            int RecordInserted = 0;

            if (newPath!=null && !Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

           
            if (importFile != null && importFile.Length > 0) 
            {
                var extension = Path.GetExtension(importFile.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    TempData["message"] = NotificationSystem.AddMessage("نوع الملف خطأ", "danger");
                    return View();
                }
                else
                {
                    string sFileExtension = Path.GetExtension(importFile.FileName).ToLower();
                    ISheet sheet;
                    string filename = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + sFileExtension;
                    string fullPath = Path.Combine(newPath, filename);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        importFile.CopyTo(stream);
                        stream.Position = 0;
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                            sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                            sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                        }
                        IRow headerRow = sheet.GetRow(0); //Get Header Row

                        int cellCount = headerRow.LastCellNum;


                        using (var dbCtxTransaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                for (int j = 0; j < cellCount; j++)
                                {
                                    NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                                }

                                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                                {

                                    // catch each row in loop
                                    IRow row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;


                                    #region Substitute
                                    Substitute substitute = new Substitute();
                                    if (row.GetCell(0) != null && !String.IsNullOrEmpty(row.GetCell(0).ToString()))
                                    {
                                        substitute.Day = row.GetCell(0).ToString().Trim();
                                        substitute.SubstituteDate = DateTime.Parse(row.GetCell(1).ToString().Trim());
                                        substitute.FullName = row.GetCell(2).ToString().Trim();
                                        substitute.MilitaryNumber = row.GetCell(3) != null && !String.IsNullOrEmpty(row.GetCell(3).ToString()) ? row.GetCell(3).ToString() : "";
                                        substitute.CategoryId = Int32.Parse(row.GetCell(4).ToString().Trim());
                                        substitute.IsVacation = Int32.Parse(row.GetCell(5).ToString().Trim()) == 1 ? true : false;

                                        _context.Substitutes.Add(substitute);
                                        _context.SaveChanges();
                                    }
                                    #endregion
                                    RecordInserted++;
                                }
                                #region cleanup
                                //cleanup
                                GC.Collect();
                                GC.WaitForPendingFinalizers();

                                //deleting file from tmp folder after being used
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                                #endregion

                                dbCtxTransaction.Commit();
                                //  TempData["message"] = NotificationSystem.AddMessage("تم إدخال المعلومات ", "success");
                                TempData["message"] = NotificationSystem.AddMessage(RecordInserted+" records imported" ,  "success");

                                return View();
                            }
                            catch (Exception Ex)
                            {
                                dbCtxTransaction.Rollback();
                                TempData["message"] = NotificationSystem.AddMessage(Ex.Message.ToString(), "danger");
                                return View();
                                throw;
                            }
                        }
                    }
                }
                
            }
            else
            {
                TempData["message"] = NotificationSystem.AddMessage("يجب اختيار الملف", "danger");
            }
            return View();
        }




        // POST: Substitutes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Day,SubstituteDate,FullName,MilitaryNumber,IsVacation,CategoryId")] Substitute substitute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(substitute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", substitute.CategoryId);
            return View(substitute);
        }

        // GET: Substitutes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var substitute = await _context.Substitutes.FindAsync(id);
            if (substitute == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", substitute.CategoryId);
            return View(substitute);
        }

        // POST: Substitutes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Day,SubstituteDate,FullName,MilitaryNumber,IsVacation,CategoryId")] Substitute substitute)
        {
            if (id != substitute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(substitute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubstituteExists(substitute.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", substitute.CategoryId);
            return View(substitute);
        }

        // GET: Substitutes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var substitute = await _context.Substitutes
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (substitute == null)
            {
                return NotFound();
            }

            return View(substitute);
        }

        // POST: Substitutes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var substitute = await _context.Substitutes.FindAsync(id);
            _context.Substitutes.Remove(substitute);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubstituteExists(int id)
        {
            return _context.Substitutes.Any(e => e.Id == id);
        }
    }
}
