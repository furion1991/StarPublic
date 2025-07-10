'use client'

import { ContractPrize } from '@/entities/contracts'
import { ContractPrizeActions } from '@/features/contracts'
import { type Loot, LootItem } from '@/entities/loot'
import { Button } from '@/shared/ui'
import { useSellItems } from '@/features/inventory'
import { useAuth, useUser } from '@/shared/hooks'

import classes from './ContractResult.module.scss'

type PrizeItem = {
  inventoryRecordId: string
  isItemActive: boolean
  itemState: number
  item: Loot
}

type ItemIds = {
  inventoryItemId: string
  itemId: string
}

type ContractResultProps = {
  prizeItem: PrizeItem
  onItemAddToContract: (itemIds: ItemIds) => void
  onContractRestart: () => void
}

export const ContractResult = ({
  prizeItem,
  onContractRestart,
  onItemAddToContract
}: ContractResultProps) => {
  const { user } = useUser()
  const { mutate: sellItem } = useSellItems()

  const inventoryItemsList = user
    ? user.userInventory.availableInventoryItems
        .map(({ id, itemId, itemDto }) => {
          const { game, name, image, sellPrice, rarity } = itemDto

          return {
            id,
            itemId,
            game,
            name,
            image,
            sellPrice,
            rarity
          }
        })
        .slice(0, 6)
    : []

  return (
    <>
      <div className={classes.prize}>
        <ContractPrize prizeImg={prizeItem.item.image} />
      </div>

      <div className={classes.prizeActions}>
        <ContractPrizeActions
          prizeSellPrice={prizeItem.item.sellPrice}
          onContractRetry={() => {
            onContractRestart()
          }}
          onItemSell={() => {
            sellItem([prizeItem.inventoryRecordId])
          }}
        />
      </div>

      <h2>Доступные для контрактов предметы</h2>

      <ul className={classes.availableItemsForContract}>
        {inventoryItemsList.map(({ id, itemId, name, image, sellPrice, rarity }) => {
          return (
            <li key={id}>
              <LootItem
                className={classes.lootItem}
                name={name}
                image={image}
                price={sellPrice}
                rarity={rarity}
              />

              <div className={classes.actionBox}>
                <Button
                  onClick={() => {
                    onItemAddToContract({ inventoryItemId: id, itemId })
                    onContractRestart()
                  }}
                >
                  Добавить в контракт
                </Button>
              </div>
            </li>
          )
        })}
      </ul>
    </>
  )
}
