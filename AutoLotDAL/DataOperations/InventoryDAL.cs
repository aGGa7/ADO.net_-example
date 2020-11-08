using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using AutoLotDAL.Models;

namespace AutoLotDAL.DataOperations
{
  public  class InventoryDAL
    {
        private readonly string _connectionString;
        private SqlConnection _sqlconnection = null; //переменная для хранения подключения. Предоставляет возможность подключения и отключения от
        //хранилища данных.Объекты подключения также обеспечивают доступ к связанному объекту транзакции

        public InventoryDAL():this (@"Data Source = (localdb)\mssqllocaldb; Integrated Security = true; Initial Catalog = AutoLot") // передача конструктору строки подключения, 
        { }

        public InventoryDAL(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        /*
         * Объекты подключений .NET обеспечиваются форматированной строкой подключения, которая содержит несколько пар “имя-значение”, разделенных точками
         * с запятой. Такая информация идентифицирует имя машины, к которой нужно подключиться, требуемые настройки безопасности, имя базы данных на
         * машине и другие специфичные для поставщика сведения. Из приведенного выше кода можно сделать вывод, что имя Initial Catalog относится к 
         * базе данных, с которой необходимо установить сеанс. Имя Data Source идентифицирует имя машины, где находится база данных. Мы применяем
         * строку (localdb) \mssqllocaldb, которая относится к версии SQL Server Express, установленной вместе с Visual Studio 2017. В случае использования
         * другого экземпляра свойство определяется как имя_машины\экземпляр. Например, MYSERVER\SQLSERVER2016 обозначает сервер по имени MYSERVER с
         * экземпляром по имени SQLSERVER2016. Если применяется локальная машина разработки, тогда в качестве имени сервера указывается точка (.) или 
         * конструкция (local) . В случае стандартного экземпляра SQL Server имя экземпляра опускается. Скажем, если база данных AutoLot была создана в 
         * стандартном экземпляре Microsoft SQL Server на локальном компьютере, то следует использовать Data Source=(local).
         * */
        private void OpenConnection () //метод для открытия подключения
        {
            _sqlconnection = new SqlConnection { ConnectionString = _connectionString };
            _sqlconnection.Open();
        }

        private void  CloseConnection () // метод для закрытия подключения
        {
            if (_sqlconnection?.State != ConnectionState.Closed)
                _sqlconnection?.Close();
        }
        public List<Car> GetAllInventory () // Метод GetAllInventory () возвращает экземпляр List<Car>, представляющий все данные в таблице Inventory:
        {
            OpenConnection();
            List<Car> inventory = new List<Car>();
            string sql = "Select * From Test"; //подготовка sql оператора, Test имя таблицы
            /*
             * Подобно другим объектным моделям доступа к данным объекты команд позволяют программно манипулировать с операторами SQL, 
             * хранимыми процедурами и параметризированными запросами. Объекты команд также обеспечивают доступ к типу чтения данных поставщика
             * данных посредством перегруженного метода ExecuteReader ()
             * */
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection)) //подготовка объекта комманды
            {
                command.CommandType = CommandType.Text; //SQL-запрос
                                                        //Тип SqlCommand (производный от DbCommand) является объектно-ориентированным представлением SQL запроса, имени таблицы или 
                                                        //хранимой процедуры. Тип команды указывается с использованием свойства CommandType, которое принимает любое значение из перечисления CommandType
                SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection); // Получить объект чтения данных с помощью ExecuteReader(). Свойство
               // CommandBehavior класса DataReader настроено на автоматическое закрытие подключения,  когда закрывается объект чтения данных.
                /* SqlDataReader Предоставляет доступ к данным в направлении только вперед, допускающий только чтение, с использованием курсора на стороне сервера
                 * Объекты чтения данных получаются из объекта команды с применением вызова ExecuteReader (). Объект чтения данных представляет 
                 * текущую запись, прочитанную из базы данных. Он имеет метод индексатора (например, синтаксис [ ] в языке С#), который позволяет
                 * обращаться к столбцам текущей записи. Доступ к конкретному столбцу возможен либо по имени, либо по целочисленному индексу, начинающемуся с нуля.
                */
                while (dataReader.Read()) //с помощью метод Read() которого выясняется, когда достигнут конец записей(в случае чего он возвращает false).
                {
                    inventory.Add(new Car
                    {
                        Carld = (int)dataReader["CarID"], //читай выше
                        Color = (string)dataReader["Color"],
                        Make = (string)dataReader["Make"],
                        PetName = (string)dataReader["PetName"]
                    }
                        );
                    
                }
                dataReader.Close();
            }
            return inventory;
        }
        public Car GetCar (int id) //метод выборки получает одиночную запись об автомобиле на основе значения Car Id
        {
            OpenConnection();
            Car car = null;
            string sql = $"Select * From Test where CarID={id}";
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection))
            {
                command.CommandType = CommandType.Text;
                SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (dataReader.Read())
                {
                    car = new Car
                    {
                        Carld = (int)dataReader["CarID"], //читай выше
                        Color = (string)dataReader["Color"],
                        Make = (string)dataReader["Make"],
                        PetName = (string)dataReader["PetName"]
                    };
                }
                dataReader.Close();
            }
            return car;
        }
        /*
          * Вставка новой записи в таблицу Inventory сводится к построению SQL-оператора Insert (на основе пользовательского ввода), открытию подключения, вызову метода
          * ExecuteNonQuery () с применением объекта команды и закрытию подключения. Увидеть вставку в действии можно, добавив к типу InventoryDAL открытый метод по
          * имени Insert Auto (), который принимает четыре параметра, отображаемые на четыре столбца таблицы Inventory (Carld, Color, Make и PetName). Указанные аргументы
          * используются при форматировании строки для вставки новой записи. И, наконец, для выполнения итогового оператора SQL применяется объект SqlConnection
          */
        public void InsertAuto  (string color, string make, string petname) //метод вставки новой записи, в качестве запроса используется строковый литерал
        {
            OpenConnection();
            string sql = $"Insert Into Test (Make, Color, PetName) Values ('{make}' , '{color}', '{petname}')";// Сформатировать и выполнить оператор SQL, Test имя таблицы
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection))// Выполнить, используя наше подключение
            {
                command.CommandType = CommandType.Text;
                /*
                 * если необходимо отправить операторы SQL, которые в итоге модифицируют таблицу данных (или любой другой отличающийся от запроса оператор SQL, такой как создание таблицы 
                 * либо выдача разрешений), то потребуется вызов метода ExecuteNonQuery () объекта команды. В зависимости от формата текста команды указанный единственный метод выполняет вставки, 
                 * обновления и удаления. Метод ExecuteNonQuery () возвращает значение int, которое представляет количество строк, затронутых операторами, а не новый набор записей.
                 * */
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }
        //еще одна версия метода InsertAuto (), которая принимает в качестве параметра модель Саг и использует параметризованный запрос для вставки новой записи
        /*
         * В параметризированных запросах параметры SQL являются объектами, а не простыми порциями текста. Трактовка запросов SQL в более объектно-ориентированной манере помогает 
         * сократить количество опечаток (учитывая, что свойства строго типизированы). Вдобавок параметризированные запросы обычно выполняются значительно быстрее запросов в виде строковых
         * литералов, т.к. они подвергаются разбору только однажды (а не каждый раз, когда строка с запросом SQL присваивается свойству CommandText). Параметризированные запросы также содействуют 
         * в защите против атак внедрением в SQL (хорошо известная проблема безопасности доступа к данным). Для поддержки параметризированных запросов объекты команд ADO.NET содержат коллекцию
         * индивидуальных объектов параметров. По умолчанию коллекция пуста, но в нее можно вставить любое количество объектов параметров, которые отображаются на параметры-заполнители в запросе SQL.
         * Чтобы ассоциировать параметр внутри запроса SQL с членом коллекции параметров в объекте команды, параметр запроса SQL необходимо снабдить префиксом в виде символа @ 
         * (во всяком случае, когда применяется icrosoft SQL Server; не все СУБД поддерживают такую систему обозначений).
         * */
        public void InsertAuto (Car car)
        {
            OpenConnection();
            string sql = "Insert Into Test" + "(Make, Color, PetName) Values" +  "(@Make, @Color, @PetName)"; // Обратите внимание на "заполнители" в запросе SQL.
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection))// Выполнить, используя наше подключение
            {
                SqlParameter parameter = new SqlParameter // Заполнить коллекцию параметров. Тип SqlParameter применяется для сопоставления с каждым заполнителем за счет указания в 
                                                          //свойстве ParameterName его имени и предоставления других деталей(например, значения, типа данных и размера) в строго типизированной манере.
                {
                    ParameterName = "@Make", //Получает или устанавливает имя DbParameter
                    Value = car.Make, //Получает или устанавливает значение параметра
                    SqlDbType = SqlDbType.Char, //Получает или устанавливает собственный тип данных параметра, представленный как тип данных CLR
                    Size = 10 //Получает или устанавливает максимальный размер данных в байтах для параметра(полезно только для текстовых данных)
                };
                command.Parameters.Add(parameter); // После того, как объект параметра готов, вызовом метода Add() он добавляется в коллекцию объекта команды

                parameter = new SqlParameter // описание см выше
                {
                    ParameterName = "@Color", 
                    Value = car.PetName,
                    SqlDbType = SqlDbType.Char, 
                    Size = 10 
                };
                command.Parameters.Add(parameter);

                parameter = new SqlParameter // описание см выше
                {
                    ParameterName = "@PetName",
                    Value = car.PetName,
                    SqlDbType = SqlDbType.Char, 
                    Size = 10 
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery(); //описание см выше
            }
            CloseConnection();
        }

        /*
         * Удаление существующей записи не сложнее вставки новой записи. В отличие от метода InsertAuto () на этот раз вы узнаете о важном блоке try/catch, который обрабатывает
         * возможную попытку удалить запись об автомобиле, уже заказанном кем-то из таблицы Customers. Стандартные параметры INSERT и UPDATE для внешних ключей
         * по умолчанию предотвращают удаление зависимых записей в связанных таблицах. Когда предпринимается попытка подобного удаления, генерируется исключение
         * Sql Except ion. В реальной программе была бы предусмотрена логика обработки такой ошибки, но в рассматриваемом примере мы просто генерируем новое исключение.
         * */
        public void DeleteCar (int id)
        {
            OpenConnection();
            string sql = $"Delete from Test where CarID = '{id}'";
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection)) //Получить идентификатор автомобиля, подлежащего удалению, и удалить запись о нем.
            {
                try
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Sorry! That car is on order!", ex); 
                    throw error;
                }
            }
            CloseConnection();
        }
        /*
         * метод, который дает вызывающему коду возможность обновить дружественное имя указанного автомобиля
         * */
        public void UpdateCar (int id, string newpetname)
        {
            OpenConnection();
            string sql = $"Update Test Set PetName = '{newpetname}' Where CarID = '{id}'"; //// Получить идентификатор автомобиля для модификации дружественного имени,
            using (SqlCommand command = new SqlCommand(sql, _sqlconnection))
            {
                command.ExecuteNonQuery();           
            }
            CloseConnection();
        }
        /*
         * хранимая процедура представляет собой именованный блок кода SQL, сохраненный в базе данных. Хранимые процедуры можно конструировать так,
         * чтобы они возвращали набор строк либо скалярных типов данных или выполняли еще какие-то осмысленные действия (например, вставку, обновление или удаление записей);
         * в них также можно предусмотреть любое количество необязательных параметров. Конечным результатом будет единица работы, которая ведет себя подобно типичной
         * функции, но только находится в хранилище данных, а не в двоичном бизнес-объекте. В текущий момент в базе данных AutoLot определена единственная хранимая процедура по имени Get Pet Name.
         * */

        public string LookUpPetName(int carld)
        {
            OpenConnection();
            string carPetName;
            /*
             * Когда объекту команды необходимо сообщить о том, что он будет вызывать хранимую процедуру, потребуется указать имя этой процедуры (в аргументе конструктора или в свойстве CommandText) 
             * и установить свойство CommandType в CommandType. StoredProcedure. (В противном случае возникнет исключение времени выполнения, т.к. по умолчанию объект команды ожидает оператор SQL.)
             * */
            using (SqlCommand command = new SqlCommand ("GetPetName", _sqlconnection)) // Установить имя хранимой процедуры.
            {
                command.CommandType = CommandType.StoredProcedure; //читай выше
                //все объекты параметров добавляются в коллекцию параметров объекта команды:
                SqlParameter param = new SqlParameter // Входной параметр.
                {
                    ParameterName = "@carID",
                    SqlDbType = SqlDbType.Int,
                    Value = carld,
                    Direction = ParameterDirection.Input //свойство Direction объекта параметра позволяет указать направление движения каждого параметра, передаваемого хранимой процедуре
                                                        //(например, входной параметр, выходной параметр, входной / выходной параметр или возвращаемое значение).
                };
                command.Parameters.Add(param);
                param = new SqlParameter //Выходной параметр,
                {
                    ParameterName = "@petName",
                    SqlDbType = SqlDbType.Char,
                    Size = 10,
                    Direction = ParameterDirection.Output //см выше
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery(); // Выполнить хранимую процедуру
                /*
                 * После того, как хранимая процедура, запущенная вызовом метода ExecuteNonQuery () завершила работу, можно получить значение выходного параметра, 
                 * просмотрев коллекцию параметров объекта команды и применив соответствующее приведение
                 * */
                carPetName = (string)command.Parameters["@PetName"].Value; // Возвратить выходной параметр.
                CloseConnection();
            }
            return carPetName;
        }

        /*перемещение рискованного клиента из таблицы Customers в таблицу CreditRisks должно происходить под недремлющим оком транзакционного контекста (в конце концов, вы ведь хотите запомнить 
         * имена некредитоспособных клиентов). В частности, вам необходимо гарантировать, что либо текущие кредитные риски будут успешно удалены из таблицы Customers и добавлены в таблицу
         * CreditRisks, либо ни одна из упомянутых операций базы данных не выполнится.
         * */

        public void ProcessCreditRisk (bool throwEX, int custID) //метод для осуществления транзакции
        {
            OpenConnection();
            // Первым делом найти текущее имя по идентификатору клиента,
            string firname;
            string lasname;
            var cmdSelect = new SqlCommand($"Select * from Customers where CustID = {custID}", _sqlconnection);
            /*
             * объекты чтения данных представляют поток данных, допускающий только чтение в прямом направлении, который возвращает по одной записи за раз. Таким образом, объекты чтения
             * данных полезны, только когда лежащему в основе хранилищу данных отправляются SQL-операторы выборки. Объекты чтения данных удобны, если нужно быстро пройти по большому объему 
             * данных без необходимости иметь их представление в памяти. Тем не менее, имейте в виду, что объекты чтения данных (в отличие от объектов адаптеров данных, которые рассматриваются 
             * позже) удерживают подключение к источнику данных открытым до тех пор, пока вы его явно не закроете. 
             * */
            using (var dataReader = cmdSelect.ExecuteReader()) // Объекты чтения данных получаются из объекта команды с применением вызова ExecuteReader().
            {
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    firname = (string)dataReader["FirstName"]; //Объект чтения данных представляет текущую запись, прочитанную из базы данных. Он имеет метод индексатора(например, синтаксис[] в языке С#),
                    lasname = (string)dataReader["LastName"]; // которыi позволяет обращаться к столбцам текущей записи. Доступ к конкретному столбцу возможен либо по имени, либо по целочисленному индексу, начинающемуся с нуля.
                }
                else
                {
                    CloseConnection();
                    return;
                }
            }
            var cmdRemove = new SqlCommand($"Delete from Customers where CustID = {custID}", _sqlconnection); //объект команды удаляющую запись из таблицы Customers
            var cmdInsert = new SqlCommand($"Insert Into CreditRisks" + $"(FirstName, LastName) Values ('{firname}', '{lasname}')", _sqlconnection); //вставляем новую запись в таблицу CreditRisks
            SqlTransaction tx = null;
            try
            {
                /*Обратите внимание на применение двух объектов SqlCommand для представления каждого шага транзакции, которая будет запущена. После получения имени и фамилии 
                 * клиента на основе входного параметра custld с помощью метода BeginTransaction () объекта подключения можно получить допустимый объект SqlTransaction. Затем (что 
                 * очень важно) потребуется привлечь к участию каждый объект команды, присвоив его свойству Transaction полученного объекта транзакции. Если этого не сделать, то логика 
                 * вставки и удаления не будет находиться в транзакционном контексте.
                 * */
                tx = _sqlconnection.BeginTransaction();
                cmdInsert.Transaction = tx; //включить команды в транзакцию
                cmdRemove.Transaction = tx;
                cmdInsert.ExecuteNonQuery(); // Выполнить команды,
                cmdRemove.ExecuteNonQuery();
                if (throwEX) //для симуляции ошибки, чтобы преравть транзакцию
                {
                    throw new Exception("Sorry! Database error! Tx failed...");
                }
                /*Если исключение не было сгенерировано, тогда оба шага будут зафиксированы в таблицах базы данных в результате вызова Commit (). */
                tx.Commit(); // Зафиксировать транзакцию!
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Любая ошибка приведет к откату транзакции.
                // Использовать условную операцию для проверки на предмет null,
                tx?.Rollback();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
