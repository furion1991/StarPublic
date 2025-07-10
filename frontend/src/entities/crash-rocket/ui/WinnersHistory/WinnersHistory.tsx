import cn from 'classnames'

import classes from './WinnersHistory.module.scss'
import Image from 'next/image'

enum LootRarity {
  COMMON = 0,
  RARE = 1,
  SUPER_RARE = 2,
  EPIC = 3,
  MYTHICAL = 4,
  LEGENDARY = 5
}

export const WinnersHistory = () => {
  const winners = [
    { id: 0, nickname: 'JetRocket 1', multiplier: 50, bet: 971, prize: 3750, rarity: 0 },
    { id: 1, nickname: 'JetRocket 2', multiplier: 51, bet: 972, prize: 3749, rarity: 1 },
    { id: 2, nickname: 'JetRocket 3', multiplier: 52, bet: 973, prize: 3751, rarity: 2 },
    { id: 3, nickname: 'JetRocket 4', multiplier: 53, bet: 974, prize: 3752, rarity: 3 },
    { id: 4, nickname: 'JetRocket 5', multiplier: 54, bet: 975, prize: 3753, rarity: 4 },
    { id: 5, nickname: 'JetRocket 6', multiplier: 55, bet: 976, prize: 3754, rarity: 5 },
    { id: 6, nickname: 'JetRocket 7', multiplier: 56, bet: 977, prize: 3755, rarity: 0 }
  ]

  return (
    <div className={classes.winnersHistory}>
      <div className={classes.labels}>
        <span>Игрок</span>
        <span>Коэф.</span>
        <span>Ставка</span>
        <span>Выигрыш</span>
      </div>

      <ul className={classes.rows}>
        {winners.map(({ id, nickname, multiplier, bet, prize, rarity }) => {
          return (
            <li
              key={id}
              className={cn({
                [classes.common]: rarity === LootRarity.COMMON,
                [classes.rare]: rarity === LootRarity.RARE,
                [classes.superRare]: rarity === LootRarity.SUPER_RARE,
                [classes.epic]: rarity === LootRarity.EPIC,
                [classes.mythical]: rarity === LootRarity.MYTHICAL,
                [classes.legendary]: rarity === LootRarity.LEGENDARY
              })}
            >
              <div className={classes.user}>
                <Image
                  src='/placeholders/avatar-winner.png'
                  width={32}
                  height={32}
                  alt={nickname}
                />
                <span>{nickname}</span>
              </div>

              <div>
                <span>{multiplier}</span>
              </div>

              <div>
                <span>{new Intl.NumberFormat('de-DE').format(bet)}</span>
              </div>

              <div>
                <span className={classes.prize}>
                  {new Intl.NumberFormat('de-DE').format(prize)}
                </span>
              </div>
            </li>
          )
        })}
      </ul>
    </div>
  )
}
