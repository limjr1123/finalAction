public interface ICurrencySystem
{
    bool TrySpend(int amount);      // 지불 시도하고, 성공/실패 반환
    int GetBalance();               // 현재 잔액 확인(은행에서 Balance는 균형이라는 뜻 x 잔고의 의미)
    void AddCurrency(int amount);   // 재화 획득
}