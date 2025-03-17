using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace PatikaOdev1.Controllers;
public class CustomerAgeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var customer = (Customer)validationContext.ObjectInstance;
        if (customer.DateOfBirth == DateTime.MinValue)
        {
            return new ValidationResult("Date of Birth is required");
        }
        var age = DateTime.Today.Year - customer.DateOfBirth.Year;
        if (age < 18 || age > 60)
        {
            return new ValidationResult("Age must be between 18 and 60");
        }
        if (customer.DateOfBirth >= DateTime.Today)
        {
            return new ValidationResult("Date of Birth can not be greater than today");
        }
        return ValidationResult.Success;
    }
}
public class Customer
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(maximumLength: 10, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 10 characters")]
    public string Name { get; set; }
    [Required]
    [StringLength(maximumLength: 10, MinimumLength = 3, ErrorMessage = "Surname must be between 3 and 10 characters")]
    public string Surname { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address. Please enter a valid email address")]
    public string Email { get; set; }
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number. Please enter a valid phone number")]
    public string PhoneNumber { get; set; }
    [Required]
    [CustomerAgeAttribute]
    public DateTime DateOfBirth { get; set; }
}

[Route("api/[controller]s")]
[ApiController]
public class CustomerController : ControllerBase
{
    List<Customer> customers = new()
    {
            new Customer()
            {
                Id = 1,
                Name = "Ahmet",
                Surname = "Yılmaz",
                Email = "ahmet.yilmaz@example.com",
                PhoneNumber = "+905551234567",
                DateOfBirth = new DateTime(1990, 5, 15)
            },
            new Customer()
            {
                Id = 2,
                Name = "Elif",
                Surname = "Demir",
                Email = "elif.demir@example.com",
                PhoneNumber = "+905554567890",
                DateOfBirth = new DateTime(1995, 8, 22)
            },
            new Customer()
            {
              Id = 3,
              Name = "Mert",
              Surname = "Kaya",
              Email = "mert.kaya@example.com",
              PhoneNumber = "+905558765432",
              DateOfBirth = new DateTime(1987, 11, 3)
            },

            new Customer(){
                Id = 4,
                Name = "Ahmet",
                Surname = "Şahin",
                Email = "ahmet.sahin@example.com",
                PhoneNumber = "+905356789900",
                DateOfBirth = new DateTime(1995, 3, 12)
            },
    };

    [HttpGet]
    public ActionResult<List<Customer>> Get()
    {
        try
        {
            if (customers == null)
                return BadRequest();

            return Ok(customers);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI Post: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("NameList")]
    public ActionResult<List<Customer>> GetAllCustomerName([FromQuery] string name)
    {
        try
        {
            var result = customers.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Name).ToList();

            if (result.Count <= 0)               
                 return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI Post: {ex.Message}");
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("{id}")]
    public ActionResult<Customer> Get(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest();

            var customer = customers.SingleOrDefault(x => x.Id == id);

            if (customer == null)
                return NotFound(" Id must be between 1 and 3");

            return Ok(customer);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI Post: {ex.Message}");
            return BadRequest(ex.Message);
        }

    }

    [HttpPost]
    public ActionResult<Customer> Post([FromBody] Customer customer)
    {
        try
        {
            if (customer == null)
                return BadRequest("Customer is not null");

            // Aynı Id ile 1 de fazla kayıt olamaz
            if (customer.Id >= 1 && customer.Id <= 4)
                return NotFound("Id must be greater than 3");

            customers.Add(customer);
            return Ok(customer);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI Post: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] Customer customer)
    {
        Customer cust = new();
        try
        {
            if (customer == null)
                return BadRequest("Customer is not null");

            var result = customers.SingleOrDefault(x => x.Id == id);

            if (result == null)
                return NotFound();

            result = customers[1];
            result.Name = customer.Name != null ? customer.Name : result.Name;
            result.Surname = customer.Surname != null ? customer.Surname : result.Surname;
            result.Email = customer.Email != null ? customer.Email : result.Email;
            result.PhoneNumber = customer.PhoneNumber != null ? customer.PhoneNumber : result.PhoneNumber;
            result.DateOfBirth = customer.DateOfBirth == DateTime.MinValue ? customer.DateOfBirth : result.DateOfBirth;

            return NoContent();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI Put: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Id must be greater than 0");

            var result = customers.SingleOrDefault(x => x.Id == id);

            if (result == null)
                return NotFound();

            customers.Remove(result);
            return Ok();
        }

        catch (Exception ex)
        {
            Debug.WriteLine($" HATA FIRLATILDI: {ex.Message}");
            return BadRequest(ex.Message);
        }

    }
}
