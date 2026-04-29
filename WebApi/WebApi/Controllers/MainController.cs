using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using WebApi;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    /// <summary>
    /// Основной контроллер API
    /// </summary>
    public class MainController : ControllerBase
    {
        private readonly string rabbitQName = "queueApiTZ";

        private readonly ILogger<MainController> _log;
        private readonly ApplicationSettings _settings;
        private readonly IConnection _connection;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public MainController(ILogger<MainController> logger, IOptions<ApplicationSettings> settings, IConnection connection)
        {
            _log = logger;
            _settings = settings.Value;
            _connection = connection;
        }

        /// <summary>
        /// Поставить задание из Rabbit
        /// </summary>
        [HttpGet("AddRabbitTask")]
        public async Task<IActionResult> AddRabbitTask([FromQuery] DataForUserTaskFormRabbit data)
        {
            // http://localhost:5245/api/main/addrabbittask

            _log.LogInformation("Запущен метод {method}: ", nameof(AddRabbitTask));

            var message = new
            {
                Task = data.Task,
                Title = data.Title,
                Description = data.Description,
                Status = data.Status
            };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            using var channel = _connection.CreateModel();
            {
                var arguments = new Dictionary<string, object>();
                arguments.Add("x-message-ttl", 600000); // 600 секунд живет очередь
                channel.QueueDeclare(rabbitQName, 
                                           durable: true,
                                           exclusive: true,
                                           autoDelete: false,
                                           arguments);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = false;
                properties.ContentType = "text/plain";
                properties.DeliveryMode = 2;
                properties.Expiration = "120000"; // 120 секунд живет сообщение
                
                channel.BasicPublish(exchange: "",
                                       routingKey: rabbitQName,
                                       basicProperties: properties,
                                       body: body);
            }
            return Ok($"AddRabbitTask ({DateTime.Now}) = {message}");
        }

        /// <summary>
        /// Выполнить задание из Rabbit
        /// </summary>
        [HttpGet("ExeRabbitTask")]
        public async Task<IActionResult> ExeRabbitTask()
        {
            // http://localhost:5245/api/main/exerabbittask

            _log.LogInformation("Запущен метод {method}: ", nameof(AddRabbitTask));

            string message = string.Empty;

            using var channel = _connection.CreateModel();
            {
                var result = channel.BasicGet(rabbitQName, true); // true — автоматическое подтверждение
                if (result != null)
                {
                    var body = result.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);
                    channel.BasicAck(result.DeliveryTag, false); // Подтверждение получения
                }   
            }
            return Ok($"ExeRabbitTask = {message}");
        }

        /// <summary>
        /// Выводит и логирует все данные из таблицы задач
        /// </summary>
        [HttpGet("GetAllData")]
        public async Task<IEnumerable<Usertask>> GetAllDataAsync()
        {
            // http://localhost:5245/api/main/getalldata

            _log.LogInformation("Запущен метод {method}: ", nameof(GetAllDataAsync));

            List<Usertask> allrecords = new();

            using (var tdc = new DbTestContext())
                allrecords = tdc.Usertasks.Where(z => z.Id>0).ToList();

            _log.LogInformation($"Вывожу все данные из таблицы задач:");
            foreach (var record in allrecords)
                _log.LogInformation($"Id={record.Id} Title={record.Title} Description={record.Description} Status={record.Status} CreatedAt={record.Dtcreate} UpdatedAt={record.Dtupdate}");
            _log.LogInformation($"");

            return allrecords;
        }

        /// <summary>
        /// Добавляет пользователя и задачу в таблицу задач
        /// </summary>
        [HttpGet("AddTask")]
        public async Task<IActionResult> AddTask([FromQuery] DataForUserTaskForm data)
        {
            // http://localhost:5245/api/main/addtask?Title=t1&Description=d1&Status=s1
            // http://localhost:5245/api/main/addtask?Title=t2&Description=d2&Status=s2
            // http://localhost:5245/api/main/addtask?Title=t3&Description=d3&Status=s3

            _log.LogInformation("Запущен метод {method}: ", nameof(AddTask));
            _log.LogInformation($"Title={data.Title} Description={data.Description} Status={data.Status}");

            using (var tdc = new DbTestContext())
            {
                var record = new Usertask();

                record.Title = data.Title;
                record.Description = data.Description;
                record.Status = data.Status;
                record.Dtcreate = DateOnly.FromDateTime(DateTime.Now);
                // record.Dtupdate = DateOnly.FromDateTime(DateTime.Now);

                tdc.Usertasks.Add(record);
                tdc.SaveChanges();
            }
            _log.LogInformation($"Добавлена запись - Title={data.Title} Description={data.Description} Status={data.Status}");
            return Ok($"Добавлена запись - Title={data.Title} Description={data.Description} Status={data.Status}");
        }

        /// <summary>
        /// Удаляет пользователя и задачу в таблицу задач
        /// </summary>
        [HttpGet("RemoveTask")]
        public async Task<IActionResult> RemoveTask([FromQuery] DataForUserTaskForm data)
        {
            // http://localhost:5245/api/main/removetask?Title=t1
            // http://localhost:5245/api/main/removetask?Title=t2
            // http://localhost:5245/api/main/removetask?Title=t3

            _log.LogInformation("Запущен метод {method}: ", nameof(RemoveTask));
            _log.LogInformation($"Title={data.Title}");

            using (var tdc = new DbTestContext())
            {
                var recList = tdc.Usertasks.Where(z => z.Title == data.Title)
                                 .ToList();

                if (recList.Count > 0)
                {
                    foreach (var rec in recList)
                    {
                        tdc.Usertasks.Remove(rec);
                    }
                    tdc.SaveChanges();
                    _log.LogInformation($"Удалена(ы) запись(и) - Title={data.Title}");
                    return Ok($"Удалена(ы) запись(и) - Title={data.Title}");
                }
                else
                {
                    _log.LogInformation($"Записи не найдены - Title={data.Title}");
                    return Ok($"Записи не найдены - Title={data.Title}");
                }
            }
        }

        /// <summary>
        /// Тест доступности контроллера
        /// </summary>
        [HttpGet("Ping")]
        public async Task<IActionResult> PingAsync()
        {   // http://localhost:5245/api/main/ping

            _log.LogInformation("Запущен метод {method}: ", nameof(PingAsync));
            return Ok($"Success {DateTime.Now}");
        }



        /// <summary>
        /// Входная форма для аргументов в запросе на изменения в таблице
        /// </summary>
        public class DataForUserTaskForm
        {
            [Required(ErrorMessage = "Title IS REQUIRED")]
            public string Title { get; set; } = "Empty";
            public string? Description { get; set; } = default;
            public string? Status { get; set; } = default;
        }

        /// <summary>
        /// Входная форма для аргументов в запросе на изменения в таблице
        /// </summary>
        public class DataForUserTaskFormRabbit
        {
            [Required(ErrorMessage = "Task IS REQUIRED")]
            public string Task { get; set; } = "Empty";
            [Required(ErrorMessage = "Title IS REQUIRED")]
            public string Title { get; set; } = "Empty";
            public string? Description { get; set; } = default;
            public string? Status { get; set; } = default;
        }
    }
}