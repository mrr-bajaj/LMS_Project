using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace LMS_Project.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int NoOfCopies { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }
        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public Publisher Publisher { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public int IssuedBooks { get; set; } =0;
        public bool IsAvailable { get; set; } = true;

        public Book()
        {
            if (NoOfCopies - IssuedBooks > 0)
                IsAvailable = true;
            else
                IsAvailable = false;
        }
    }
}
