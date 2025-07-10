'use client'

import Link from 'next/link'
import cn from 'classnames'
import Image from 'next/image'
import { useState } from 'react'

import { LootItem } from '@/entities/loot'
import { Button, NoDataPanel } from '@/shared/ui'

import { InventoryItemStatus, type InventoryItem } from '@/entities/user'

import classes from './InventoryLootList.module.scss'

type InventoryLootListProps = {
  isLoading: boolean
  lootItems: InventoryItem[]
  onItemSell?: (itemInventoryId: string) => void
  onItemWithdraw?: (itemInventoryId: string) => void
}

export const InventoryLootList = ({
  isLoading,
  lootItems,
  onItemSell,
  onItemWithdraw
}: InventoryLootListProps) => {
  const [soldItemsIds, setSoldItemsIds] = useState<string[]>([])

  if (isLoading) return null

  if (!lootItems.length) {
    return (
      <NoDataPanel
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
    <ul className={classes.inventoryLootList}>
      {lootItems.map(({ id, itemRecordState, itemDto }) => {
        const { rarity, name, sellPrice, image } = itemDto
        let stateIcon = null

        const itemStatusesWithoutActions = [InventoryItemStatus.SOLD, InventoryItemStatus.WITHDRAWN]
        const hasActions =
          onItemSell && onItemWithdraw && !itemStatusesWithoutActions.includes(itemRecordState)

        if (itemRecordState === InventoryItemStatus.SOLD) {
          stateIcon = (
            <Image src='/icons/wallet-cyan.svg' width={22.73} height={22.73} alt='Кошелек' />
          )
        }

        if (itemRecordState === InventoryItemStatus.WITHDRAWN) {
          stateIcon = (
            <Image
              src='/icons/square-arrow-up.svg'
              width={22.73}
              height={22.73}
              alt='Стрелка вверх'
            />
          )
        }

        return (
          <li
            key={id}
            className={cn(classes.inventoryLootItem, {
              [classes.withActions]: hasActions
            })}
          >
            <LootItem
              className={classes.lootItem}
              rarity={rarity}
              name={name}
              price={sellPrice}
              image={image}
              slots={{
                topLeft: stateIcon
              }}
            />

            {hasActions ? (
              <div className={classes.actions}>
                <div className={classes.lootItemActions}>
                  <Button
                    onClick={() => {
                      onItemSell(id)
                      setSoldItemsIds([...soldItemsIds, id])
                    }}
                  >
                    {!soldItemsIds.includes(id) ? 'Продать' : 'Продано'}
                  </Button>

                  <Button color='purple'>Вывести</Button>
                </div>
              </div>
            ) : null}
          </li>
        )
      })}
    </ul>
  )
}
