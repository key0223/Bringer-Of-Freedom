using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessages
{
    public const float MessageDuration = 2;
    public const string MESSAGE_PHASE1_START = "잊힌 장군의 동상이 깨어납니다!\r\n[주요 위협 : 맹독의 이무기와 파멸의 검귀]";
    public const string MESSAGE_HIT_GREEN = "동상에 속박된 맹독의 이무기와 파멸의 검귀를 제압하십시오.";
    public const string MESSAGE_PHASE1_GIMMICK_START = "이무기가 맹독의 포자를 모읍니다! \r\n검귀의 약점을 노려 무력화하십시오.";
    public const string MESSAGE_RETRIEVE_BALL = "무력화된 검귀의 입에서 기운을 흡수하십시오.\r\n[F] 키를 눌러 여의주를 회수!";
    public const string MESSAGE_PHASE1_END = "맹독의 이무기와 파멸의 검귀를 봉인했습니다.";

    public const string MESSAGE_PHASE2_START = "동상의 중단부가 개방됩니다.\r\n[새로운 위협 : 작열의 화신과 냉혹한 쌍두!]";
    public const string MESSAGE_PHASE2_START2 = "원소 공격의 잔류 흔적입니다.\r\n[F] 키를 눌러 여의주의 기운을 모으십시오.";
    public const string MESSAGE_HIT_BLACK = " 화신과 쌍두의 원소 공격을 역이용하여 봉인하십시오.";
    public const string MESSAGE_PHASE2_GIMMICK_START = " 여의주의 기운이 가득 찼습니다.\r\n[G] 키를 눌러 기운을 방출해 용들을 제압하십시오!";
    public const string MESSAGE_PHASE2_END = "작열의 화신과 냉혹한 쌍두의 봉인을 완료했습니다. \r\n천둥의 패왕이 전면에 나설 준비를 합니다!";

    public const string MESSAGE_PHASE3_START = " 천둥의 패왕이 전면에 나섭니다!\r\n최종 봉인을 위해 동상의 최상단으로 오르십시오.";
    public const string MESSAGE_HIT_YELLOW = "패왕의 맹공을 뚫고 여의주를 타격하여 무력화하십시오.";
    public const string MESSAGE_PHASE3_END = "패왕을 처단하고 잊힌 장군의 동상을 완전히 봉인했습니다.";
    public const string MESSAGE_INTERACTION_HAT = "최후의 봉인!\r\n[F] 키를 눌러 동상의 투구와 상호작용하십시오.";
}
