using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.RequestArgsForm
{
    /// <summary>
    /// Входная форма для аргументов в запросе на изменения в таблице для Rabbit
    /// </summary>
    public class DataForUserTaskFormRabbit
    {
        /// <summary>
        /// Задание
        /// </summary>
        [Required(ErrorMessage = "Task IS REQUIRED")]
        public string Task { get; set; } = "Empty";
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
