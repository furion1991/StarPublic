import Link from 'next/link'
import cn from 'classnames'

import { LootItem } from '@/entities/loot'
import { Button, NoDataPanel } from '@/shared/ui'

import classes from './InventoryLootList.module.scss'

type LootItem = {
  inventoryItemId: string
  rarity: number
  game: string
  name: string
  sellPrice: number
  image: string
}

type InventoryLootListProps = {
  isLoading: boolean
  lootItems: LootItem[]
  lootItemActions?: React.ReactNode
}

export const InventoryLootList = ({
  isLoading,
  lootItems,
  lootItemActions
}: InventoryLootListProps) => {
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
      {lootItems.map(({ inventoryItemId, rarity, name, game, sellPrice, image }) => {
        // let stateIcon = null

        // if (state === 'sold') {
        //   stateIcon = (
        //     <Image src='/icons/wallet-cyan.svg' width={22.73} height={22.73} alt='Кошелек' />
        //   )
        // }

        // if (state === 'withdrawn') {
        //   stateIcon = (
        //     <Image
        //       src='/icons/square-arrow-up.svg'
        //       width={22.73}
        //       height={22.73}
        //       alt='Стрелка вверх'
        //     />
        //   )
        // }

        return (
          <li
            key={inventoryItemId}
            className={cn(classes.inventoryLootItem, {
              [classes.withActions]: Boolean(lootItemActions)
            })}
          >
            <LootItem
              className={classes.lootItem}
              rarity={rarity}
              name={name}
              price={sellPrice}
              image={image}
              // slots={{
              //   topLeft: stateIcon
              // }}
            />

            {lootItemActions ? <div className={classes.actions}>{lootItemActions}</div> : null}
          </li>
        )
      })}
    </ul>
  )
}
