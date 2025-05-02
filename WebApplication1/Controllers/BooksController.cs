using IRepositoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApplication1.Dtos.Book;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly string[] BookIncludes = { "Trip", "ApplicationUser" };

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Removed invalid field initializer referencing _unitOfWork and createBookDto

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetBooks()
        {
            var books = await _unitOfWork.Books.GetAllWith(BookIncludes).ToListAsync();
            var bookDtos = books.Select(b => new BookDetailDto
            {
                BookId = b.BookId,
                TripId = b.TripId,
                TripName = b.Trip?.Name,
                ApplicationUserId = b.ApplicationUserId,
                DateBook = b.DateBook,
                StartComingDate = b.StartComingDate,
                EndComingDate = b.EndComingDate,
                NumberDays = b.NumberDays,
                NumberPeople = b.NumberPeople,
                AmountMoney = b.AmountMoney,

            });

            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailDto>> GetBook(string id)
        {
            var book = _unitOfWork.Books.GetdetailWith(BookIncludes, b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = new BookDetailDto
            {
                BookId = book.BookId,
                TripId = book.TripId,
                TripName = book.Trip?.Name,
                ApplicationUserId = book.ApplicationUserId,
                DateBook = book.DateBook,
                StartComingDate = book.StartComingDate,
                EndComingDate = book.EndComingDate,
                NumberDays = book.NumberDays,
                NumberPeople = book.NumberPeople,
                AmountMoney = book.AmountMoney,
            };

            return bookDto;
        }

        [HttpPost]
        public async Task<ActionResult<BookDetailDto>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                // 1. Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // 2. Validate dates
                if (createBookDto.StartComingDate >= createBookDto.EndComingDate)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid Dates",
                        Detail = "End date must be after start date",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // 3. Find trip by name (corrected to use FindAll)
                var trip = _unitOfWork.Trips.FindAll(t => t.Name == createBookDto.TripName).FirstOrDefault();
                if (trip == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Trip Not Found",
                        Detail = $"Trip with name '{createBookDto.TripName}' doesn't exist",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                // 4. Calculate booking details
                var numberOfDays = createBookDto.NumberDays ??
                                 (createBookDto.EndComingDate - createBookDto.StartComingDate).Days;

                var amountMoney = createBookDto.AmountMoney ??
                                (trip.Money * createBookDto.NumberPeople * numberOfDays);

                // 5. Create booking entity (using actual trip ID)
                var book = new Book
                {
                    BookId = Guid.NewGuid().ToString(),
                    TripId = trip.TripId,  // Use the actual trip ID
                    DateBook = DateTime.UtcNow,
                    StartComingDate = createBookDto.StartComingDate.Date,
                    EndComingDate = createBookDto.EndComingDate.Date,
                    NumberDays = numberOfDays,
                    NumberPeople = createBookDto.NumberPeople,
                    AmountMoney = amountMoney
                };

                // 6. Save to database with transaction
                await _unitOfWork.BeginTransactionAsync();

                _unitOfWork.Books.Addone(book);
                var recordsAffected = _unitOfWork.Compelet();

                if (recordsAffected <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "Save Failed",
                        Detail = "No records were saved to the database",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                await _unitOfWork.CommitTransactionAsync();
                var createdBook = _unitOfWork.Books.GetdetailWith(BookIncludes, b => b.BookId == book.BookId);

                // 7. Return successful response
                return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, new BookDetailDto
                {
                    BookId = book.BookId,
                    TripId = book.TripId,
                    TripName = createdBook.Trip?.Name,
                    DateBook = book.DateBook,
                    StartComingDate = book.StartComingDate,
                    EndComingDate = book.EndComingDate,
                    NumberDays = book.NumberDays,
                    NumberPeople = book.NumberPeople,
                    AmountMoney = book.AmountMoney
                });
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();

                // Extract SQL Server specific error if available
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = $"Database operation failed: {errorMessage}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Server Error",
                    Detail = $"An unexpected error occurred: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        // DELETE: api/Books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            var book = _unitOfWork.Books.Findme(b => b.BookId == id);
            if (book == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Book Not Found",
                    Detail = $"No booking found with ID: {id}",
                    Status = StatusCodes.Status404NotFound
                });
            }

            await _unitOfWork.BeginTransactionAsync();

            _unitOfWork.Books.Delete(book);
            var result = _unitOfWork.Compelet();

            if (result <= 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Delete Failed",
                    Detail = "Failed to delete the booking",
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            await _unitOfWork.CommitTransactionAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookDetailDto>> UpdateBook(string id, [FromBody] UpdateBookDto updateDto)
        {

            try
            {
                // 1. Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // 2. Validate dates
                if (updateDto.StartComingDate >= updateDto.EndComingDate)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid Dates",
                        Detail = "End date must be after start date",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // 3. Get existing book with trip and user
                var book = _unitOfWork.Books.GetdetailWith(BookIncludes, b => b.BookId == id);
                if (book == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Booking Not Found",
                        Detail = $"No booking found with ID {id}",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                // 4. Get the trip based on name
                var trip = _unitOfWork.Trips.FindAll(t => t.Name == updateDto.TripName).FirstOrDefault();
                if (trip == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Trip Not Found",
                        Detail = $"Trip with name '{updateDto.TripName}' doesn't exist",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                // 5. Calculate days and total amount
                var numberOfDays = updateDto.NumberDays ??
                                 (updateDto.EndComingDate - updateDto.StartComingDate).Days;

                var amountMoney = updateDto.AmountMoney ??
                                (trip.Money * updateDto.NumberPeople * numberOfDays);

                // 6. Update fields
                book.TripId = trip.TripId;
                book.StartComingDate = updateDto.StartComingDate.Date;
                book.EndComingDate = updateDto.EndComingDate.Date;
                book.NumberDays = numberOfDays;
                book.NumberPeople = updateDto.NumberPeople;
                book.AmountMoney = amountMoney;

                // 7. Save with transaction
                await _unitOfWork.BeginTransactionAsync();

                _unitOfWork.Books.Update(book);
                var affected = _unitOfWork.Compelet();

                if (affected <= 0)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "Update Failed",
                        Detail = "No records were updated.",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                await _unitOfWork.CommitTransactionAsync();

                return Ok(new BookDetailDto
                {
                    BookId = book.BookId,
                    TripId = book.TripId,
                    TripName = trip.Name,
                    DateBook = book.DateBook,
                    StartComingDate = book.StartComingDate,
                    EndComingDate = book.EndComingDate,
                    NumberDays = book.NumberDays,
                    NumberPeople = book.NumberPeople,
                    AmountMoney = book.AmountMoney,

                });
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackTransactionAsync();

                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = $"Database operation failed: {errorMessage}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Server Error",
                    Detail = $"An unexpected error occurred: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}

 