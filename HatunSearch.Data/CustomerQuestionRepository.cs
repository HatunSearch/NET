// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class CustomerQuestionRepository : Repository, ISelectableRepository<CustomerQuestionDTO, Guid>, IUpdatableRepository<Guid>
	{
		private const string
			selectByIdQuery =
				@"SELECT CustomerQuestion.Id, CustomerQuestion.Customer AS CustomerId, Customer.FirstName AS CustomerFirstName, Customer.MiddleName AS CustomerMiddleName,
				Customer.LastName AS CustomerLastName, Customer.EmailAddress AS CustomerEmailAddress, CustomerQuestion.[Partner], CustomerQuestion.Property AS PropertyId,
				Property.[Name] AS PropertyName, CustomerQuestion.Question, CustomerQuestion.Answer, CustomerQuestion.QuestionTimeStamp, CustomerQuestion.AnswerTimeStamp,
				CustomerQuestion.HasQuestionBeenRead, CustomerQuestion.HasAnswerBeenRead
				FROM Business.CustomerQuestion AS CustomerQuestion
				JOIN Business.Customer AS Customer ON CustomerQuestion.Customer = Customer.Id
				JOIN Business.Property AS Property ON CustomerQuestion.Property = Property.Id
				WHERE CustomerQuestion.Id = @Id",
			selectByPartnerQuery =
				@"SELECT CustomerQuestion.Id, CustomerQuestion.Customer AS CustomerId, Customer.FirstName AS CustomerFirstName, Customer.MiddleName AS CustomerMiddleName,
				Customer.LastName AS CustomerLastName, Customer.EmailAddress AS CustomerEmailAddress, CustomerQuestion.[Partner], CustomerQuestion.Property AS PropertyId,
				Property.[Name] AS PropertyName, CustomerQuestion.Question, CustomerQuestion.Answer, CustomerQuestion.QuestionTimeStamp, CustomerQuestion.AnswerTimeStamp,
				CustomerQuestion.HasQuestionBeenRead, CustomerQuestion.HasAnswerBeenRead
				FROM Business.CustomerQuestion AS CustomerQuestion
				JOIN Business.Customer AS Customer ON CustomerQuestion.Customer = Customer.Id
				JOIN Business.Property AS Property ON CustomerQuestion.Property = Property.Id
				WHERE CustomerQuestion.[Partner] = @Partner";

		public CustomerQuestionRepository() { }
		public CustomerQuestionRepository(Connector connector) : base(connector) { }

		private CustomerQuestionDTO ReadFromDataReader(IDataReader reader)
		{
			return new CustomerQuestionDTO()
			{
				Id = (Guid)reader["Id"],
				Customer = new CustomerDTO()
				{
					Id = (Guid)reader["CustomerId"],
					FirstName = reader["CustomerFirstName"] as string,
					MiddleName = reader["CustomerMiddleName"] as string,
					LastName = reader["CustomerLastName"] as string,
					EmailAddress = reader["CustomerEmailAddress"] as string
				},
				Partner = new PartnerDTO() { Id = (Guid)reader["Partner"] },
				Property = new PropertyDTO()
				{
					Id = (Guid)reader["PropertyId"],
					Name = reader["PropertyName"] as string
				},
				Question = reader["Question"] as string,
				Answer = reader["Answer"] as string,
				QuestionTimeStamp = (DateTime)reader["QuestionTimeStamp"],
				AnswerTimeStamp = reader["AnswerTimeStamp"] as DateTime?,
				HasQuestionBeenRead = (bool)reader["HasQuestionBeenRead"],
				HasAnswerBeenRead = (bool)reader["HasAnswerBeenRead"]
			};
		}
		public CustomerQuestionDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<CustomerQuestionDTO> SelectByPartner(Guid partnerId) =>
			Connector.ExecuteReader(selectByPartnerQuery, new Dictionary<string, object>() { { "Partner", partnerId } }, ReadFromDataReader);
		public int Update(Guid id, IDictionary<string, object> fields) => Update("Business.CustomerQuestion", "Id", id, fields);
	}
}