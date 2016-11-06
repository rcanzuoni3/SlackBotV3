using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class GoodShitType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "goodShit", "goodshit" }; }
		public override string Help(string commandName) { return "That's some goooooood shit"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(GoodShit);
		}

		class GoodShit : CommandHandler
		{
			public GoodShit(SlackBotV3 bot) : base(bot) { }

			public override bool Execute(SlackBotCommand command)
			{
				SlackBot.Reply(command, string.Format(":ok_hand::eyes::ok_hand::eyes::ok_hand::eyes::ok_hand::eyes::ok_hand::eyes: good shit go౦ԁ sHit:ok_hand: thats :heavy_check_mark: some good:ok_hand::ok_hand:shit right:ok_hand::ok_hand:th :ok_hand: ere:ok_hand::ok_hand::ok_hand: right:heavy_check_mark:there :heavy_check_mark::heavy_check_mark:if i do ƽaү so my selｆ :100: i say so :100: thats what im talking about right there right there (chorus: ʳᶦᵍʰᵗ ᵗʰᵉʳᵉ) mMMMMᎷМ:100: :ok_hand::ok_hand: :ok_hand:НO0ОଠＯOOＯOОଠଠOoooᵒᵒᵒᵒᵒᵒᵒᵒᵒ:ok_hand: :ok_hand::ok_hand: :ok_hand: :100: :ok_hand: :eyes: :eyes: :eyes: :ok_hand::ok_hand:Good shit"));
				return false;
			}
		}
	}
}
