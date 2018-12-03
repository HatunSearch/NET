// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;

namespace HatunSearch.Entities
{
	public sealed class CustomerQuestionDTO : DTO<Guid>
	{
		public enum QuestionStatus : byte
		{
			UnreadQuestion = 1,
			UnansweredQuestion = 2,
			UnreadAnswer = 3,
			SeenAnswer = 4
		}

		public CustomerDTO Customer { get; set; }
		public PartnerDTO Partner { get; set; }
		public PropertyDTO Property { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public DateTime QuestionTimeStamp { get; set; }
		public DateTime? AnswerTimeStamp { get; set; }
		public bool HasQuestionBeenRead { get; set; }
		public bool HasAnswerBeenRead { get; set; }

		public QuestionStatus Status
		{
			get
			{
				if (HasQuestionBeenRead)
				{
					if (Answer != null) return HasAnswerBeenRead ? QuestionStatus.SeenAnswer : QuestionStatus.UnreadAnswer;
					else return QuestionStatus.UnansweredQuestion;
				}
				else return QuestionStatus.UnreadQuestion;
			}
		}
	}
}