using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.RequestArgsForm
{
    /// <summary>
    /// Входная форма для аргументов в запросе на изменения в таблице
    /// </summary>
    public class DataForUserTaskForm
    {
        /// <summary>
        /// Имя задачи
        /// </summary>
        [Required(ErrorMessage = "Title IS REQUIRED")]
        public string Title { get; set; } = "Empty";
        /// <summary>
        /// Описание задачи
        /// </summary>
        public string? Description { get; set; } = default;
        /// <summary>
        /// Состояние задачи
        /// </summary>
        public string? Status { get; set; } = default;
    }
}
