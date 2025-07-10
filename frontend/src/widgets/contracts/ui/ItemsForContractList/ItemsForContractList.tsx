'use client'

import { useUser } from '@/shared/hooks'
import { LootItem } from '@/entities/loot'
import cn from 'classnames'

import { Button } from '@/shared/ui'

import classes from './ItemsForContractList.module.scss'

type ItemsForContractListProps = {
  itemsIdsInContact: string[]
  onItemIdAddToContract: (itemId: string) => void
  onItemIdRemoveFromContract: (itemId: string) => void
}

export const ItemsForContractList = ({
  itemsIdsInContact,
  onItemIdAddToContract,
  onItemIdRemoveFromContract
}: ItemsForContractListProps) => {
  const { user } = useUser()

  const userInventoryItems = user
    ? user.userInventory.itemsUserInventory.map(({ id, itemDto }) => {
        const { game, name, image, sellPrice, rarity } = itemDto

        return {
          id,
          game,
          name,
          image,
          sellPrice,
          rarity
        }
      })
    : []

  return (
    <div className={classes.itemsForContractList}>
      {userInventoryItems.map(({ id, name, image, sellPrice, rarity }) => {
        const isIteminContract = itemsIdsInContact.includes(id)

        return (
          <div className={classes.lootItemContainer} key={id}>
            <LootItem
              className={classes.lootItem}
              name={name}
              image={image}
              price={sellPrice}
              rarity={rarity}
            />

            <div
              className={cn(classes.actionBox, {
                [classes.active]: isIteminContract
              })}
            >
              {!isIteminContract ? (
                <Button
                  className={classes.addBtn}
                  onClick={() => {
                    onItemIdAddToContract(id)
                  }}
                >
                  Добавить в контракт
                </Button>
              ) : (
                <Button
                  className={classes.removeBtn}
                  onClick={() => {
                    onItemIdRemoveFromContract(id)
                  }}
                >
                  В контракте
                </Button>
              )}
            </div>
          </div>
        )
      })}
    </div>
  )
}
