import Image from 'next/image'

import { LootItem, LootRarityBox } from '@/entities/loot'

import type { ContractItem } from '@/entities/user'

import classes from './ContractHistoryItem.module.scss'

export const ContractHistoryItem = ({ itemsUsedOnThisContract, resultItem }: ContractItem) => {
  return (
    <div className={classes.contractHistoryItem}>
      <div className={classes.items}>
        <LootItem
          className={classes.craftResult}
          image={resultItem.image}
          rarity={resultItem.rarity}
          price={resultItem.sellPrice}
          name={resultItem.name}
        />

        <div className={classes.itemsForCraft}>
          {itemsUsedOnThisContract.map(({ id, image, rarity, name }, idx) => {
            return (
              <LootRarityBox key={`${id}-${idx}`} className={classes.itemForCraft} rarity={rarity}>
                <Image src={image} width={55} height={55} alt={name} />
              </LootRarityBox>
            )
          })}
        </div>
      </div>

      <div className={classes.contractInfo}>
        <p className={classes.craftPrice}>
          Стоимость контракта:{' '}
          <span>{itemsUsedOnThisContract.reduce((total, curr) => total + curr.sellPrice, 0)}</span>
        </p>

        <p className={classes.itemsNumber}>
          Количество предметов: <span>{itemsUsedOnThisContract.length}</span>
        </p>
      </div>
    </div>
  )
}
