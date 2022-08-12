using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS_Project.Models;
using Microsoft.AspNetCore.Http;

namespace LMS_Project.Controllers
{
    public class LendRequestsController : Controller
    {
        private readonly LMS_ProjectContext _context;
        private readonly IAccountsRepo _accountsRepo;
        private readonly ILendRepository _lendRepository;

        public LendRequestsController(LMS_ProjectContext context, IAccountsRepo accountsRepo, ILendRepository lendRepository)
        {
            _context = context;
            _accountsRepo = accountsRepo;
            _lendRepository = lendRepository;
        }

        // GET: LendRequests
        public async Task<IActionResult> Index()
        {
            var lMS_ProjectContext = _context.LendRequests.Where(b => b.LendStatus == "Requested").Include(l => l.Book).Include(l => l.User);
            return View(await lMS_ProjectContext.ToListAsync());
        }

        public async Task<IActionResult> LentList()
        {
            var lMS_ProjectContext = _context.LendRequests.Include(l => l.Book).Include(l => l.User);
            return View(await lMS_ProjectContext.ToListAsync());
        }

        public async Task<IActionResult> IssuedBooks()
        {
            var username = HttpContext.Session.GetString("username");
            var user = _accountsRepo.GetUserByName(username);
            var lMS_ProjectContext = _context.LendRequests.Where(b => b.LendStatus == "Approved" && b.UserId == user.UserId).Include(l => l.Book).Include(l => l.User);
            return View(await lMS_ProjectContext.ToListAsync());
        }

        public async Task<IActionResult> UserBookRecords()
        {
            var username = HttpContext.Session.GetString("username");
            var user = _accountsRepo.GetUserByName(username);
            var lMS_ProjectContext = _context.LendRequests.Where(b => b.UserId == user.UserId).Include(l => l.Book).Include(l => l.User);
            return View(await lMS_ProjectContext.ToListAsync());
        }

        public async Task<IActionResult> AllPastBooks()
        {
            var lMS_ProjectContext = _context.LendRequests.Include(l => l.Book).Include(l => l.User);
            return View(await lMS_ProjectContext.ToListAsync());
        }

        public ViewResult RequestToLend(int bookId)
        {
            var username = HttpContext.Session.GetString("username");
            var user = _accountsRepo.GetUserByName(username);
            var noofcopies = _context.Books.SingleOrDefault(b => b.BookId == bookId).NoOfCopies;
            if (noofcopies <= 0)
            {
                _context.Books.SingleOrDefault(b => b.BookId == bookId).IsAvailable = false;
                return View("RequestedError");
            }
            var alreadyrequested = _context.LendRequests.FirstOrDefault(b=> b.BookId == bookId && b.UserId == user.UserId && b.LendStatus == "Requested");

            if(alreadyrequested != null && alreadyrequested.LendStatus == "Requested")
            {

                return View("AlreadyRequestError");
            }
            _context.Books.SingleOrDefault(b => b.BookId == bookId).NoOfCopies--;

            LendRequest lendRequest = new LendRequest()
            {
                LendStatus = "Requested",
                LendDate = System.DateTime.Now,
                BookId = bookId,
                UserId = user.UserId,
                Book = _context.Books.SingleOrDefault(b => b.BookId == bookId),
                User = _context.Accounts.SingleOrDefault(u => u.UserId == user.UserId),
            };
            _context.LendRequests.Add(lendRequest);
            _context.SaveChanges();

            /*var lr = _context.LendRequests.Where(b => b.UserId == user.UserId);
            foreach (var item in lr)
            {
                if (item.LendStatus == "Requested")
                {

                }
            }*/

            return View("Requested");
        }

        public ActionResult ApproveBook(int lendId)
        {
            LendRequest lr =  _lendRepository.GetLendRequest(lendId);
            lr.LendStatus = "Approved";
            lr.ReturnDate = lr.LendDate.AddDays(40);
            _context.SaveChanges();
            return RedirectToAction("Index");   
        }

        public ActionResult RejectBook(int lendId)
        {
            _context.LendRequests.FirstOrDefault(b => b.LendId == lendId).LendStatus = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReturnBook(int lendId)
        {
            _context.LendRequests.FirstOrDefault(b => b.LendId == lendId).LendStatus = "Returned";
            _context.LendRequests.FirstOrDefault(b => b.LendId == lendId).FineAmount = 10 * (int)(DateTime.Now - _context.LendRequests.FirstOrDefault(b => b.LendId == lendId).LendDate).TotalDays;
            _context.LendRequests.FirstOrDefault(b => b.LendId == lendId).ReturnDate = DateTime.Now;
            _context.LendRequests.Where(b => b.LendId == lendId).Include(l => l.Book).Include(l => l.User).FirstOrDefault(b => b.LendId == lendId).Book.NoOfCopies++;
            _context.SaveChanges();
            return RedirectToAction("AllBooksList","Books");
        }

        // GET: LendRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lendRequest = await _context.LendRequests
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LendId == id);
            if (lendRequest == null)
            {
                return NotFound();
            }

            return View(lendRequest);
        }

        // GET: LendRequests/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "Password");
            return View();
        }

        // POST: LendRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LendId,LendStatus,LendDate,ReturnDate,UserId,BookId,FineAmount")] LendRequest lendRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lendRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", lendRequest.BookId);
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "Password", lendRequest.UserId);
            return View(lendRequest);
        }

        // GET: LendRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lendRequest = await _context.LendRequests.FindAsync(id);
            if (lendRequest == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", lendRequest.BookId);
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "Password", lendRequest.UserId);
            return View(lendRequest);
        }

        // POST: LendRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LendId,LendStatus,LendDate,ReturnDate,UserId,BookId,FineAmount")] LendRequest lendRequest)
        {
            if (id != lendRequest.LendId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lendRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LendRequestExists(lendRequest.LendId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", lendRequest.BookId);
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "Password", lendRequest.UserId);
            return View(lendRequest);
        }

        // GET: LendRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lendRequest = await _context.LendRequests
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LendId == id);
            if (lendRequest == null)
            {
                return NotFound();
            }

            return View(lendRequest);
        }

        // POST: LendRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lendRequest = await _context.LendRequests.FindAsync(id);
            _context.LendRequests.Remove(lendRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LendRequestExists(int id)
        {
            return _context.LendRequests.Any(e => e.LendId == id);
        }
    }
}
