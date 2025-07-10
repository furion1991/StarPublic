import Image from 'next/image'

import { LootItem, LootRarity, LootRarityBox } from '@/entities/loot'

import classes from './ContractHistoryItem.module.scss'

export const ContractHistoryItem = () => {
  const itemsForCraft = [
    { id: 0, rarity: LootRarity.COMMON },
    { id: 1, rarity: LootRarity.RARE },
    { id: 2, rarity: LootRarity.SUPER_RARE },
    { id: 3, rarity: LootRarity.EPIC },
    { id: 4, rarity: LootRarity.MYTHICAL },
    { id: 5, rarity: LootRarity.LEGENDARY },
    { id: 6, rarity: LootRarity.COMMON },
    { id: 7, rarity: LootRarity.RARE },
    { id: 8, rarity: LootRarity.SUPER_RARE },
    { id: 9, rarity: LootRarity.EPIC }
  ]

  return (
    <div className={classes.contractHistoryItem}>
      <div className={classes.items}>
        <LootItem
          className={classes.craftResult}
          rarity={LootRarity.RARE}
          price={980}
          name='Starned Corbern'
        />

        <div className={classes.itemsForCraft}>
          {itemsForCraft.map(({ id, rarity }) => {
            return (
              <LootRarityBox key={id} className={classes.itemForCraft} rarity={rarity}>
                <Image src='/placeholders/case-loot.png' width={55} height={25} alt='Оружие' />
              </LootRarityBox>
            )
          })}
        </div>
      </div>

      <div className={classes.contractInfo}>
        <p className={classes.craftPrice}>
          Стоимость контракта: <span>91.750</span>
        </p>

        <p className={classes.itemsNumber}>
          Количество предметов: <span>10</span>
        </p>
      </div>
    </div>
  )
}
