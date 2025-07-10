'use client'

import { useUser } from '@/shared/hooks'
import { LootItem } from '@/entities/loot'
import cn from 'classnames'
import Link from 'next/link'

import { Button, NoDataPanel } from '@/shared/ui'

import classes from './ItemsForContractList.module.scss'

type ItemIds = {
  itemId: string
  inventoryItemId: string
}

type ItemsForContractListProps = {
  itemsIdsInContact: ItemIds[]
  onItemIdAddToContract: (props: ItemIds) => void
  onItemIdRemoveFromContract: (props: ItemIds) => void
}

export const ItemsForContractList = ({
  itemsIdsInContact,
  onItemIdAddToContract,
  onItemIdRemoveFromContract
}: ItemsForContractListProps) => {
  const { user } = useUser()

  const userInventoryItems = user
    ? user.userInventory.availableInventoryItems.map(({ id, itemId, itemDto }) => {
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
    : []

  if (!userInventoryItems.length) {
    return (
      <NoDataPanel
        className={classes.noDataPanel}
        title='Нет предметов'
        text='Начните открывать кейсы'
        action={
          <Link href='/'>
            <Button>Открыть ›</Button>
          </Link>
        }
      />
    )
  }

  return (
    <div className={classes.itemsForContractList}>
      {userInventoryItems.map(({ id, itemId, name, image, sellPrice, rarity }) => {
        const isItemInContract = itemsIdsInContact
          .map(({ inventoryItemId }) => inventoryItemId)
          .includes(id)

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
                [classes.active]: isItemInContract
              })}
              onClick={() => {
                if (isItemInContract) {
                  onItemIdRemoveFromContract({ inventoryItemId: id, itemId })
                } else {
                  onItemIdAddToContract({ inventoryItemId: id, itemId })
                }
              }}
            >
              <Button
                className={cn({
                  [classes.addBtn]: !isItemInContract,
                  [classes.removeBtn]: isItemInContract
                })}
              >
                {!isItemInContract ? 'Добавить в контракт' : 'В контракте'}
              </Button>
            </div>
          </div>
        )
      })}
    </div>
  )
}
