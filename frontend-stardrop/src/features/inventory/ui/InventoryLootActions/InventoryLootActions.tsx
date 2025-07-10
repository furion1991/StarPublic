'use client'

import { useState } from 'react'

import { Button, ConfirmModal, Switch } from '@/shared/ui'

import { useSellItems } from '../../model/useSellItems'
import { useUser } from '@/shared/hooks'
import { InventoryItemStatus } from '@/entities/user'

import classes from './InventoryLootActions.module.scss'

export const InventoryLootActions = () => {
  const [isSellConfirmModalOpen, setSellConfirmModalOpen] = useState(false)

  const { user } = useUser()
  const { mutate: sellItems, isPending: isSellInProcess } = useSellItems({
    onSuccess: () => {
      setSellConfirmModalOpen(false)
    }
  })

  const handleAllItemsSell = () => {
    const sellableUserItemsIds = user
      ? user.userInventory.itemsUserInventory
          .filter(
            ({ itemRecordState }) =>
              ![InventoryItemStatus.SOLD, InventoryItemStatus.WITHDRAWN].includes(itemRecordState)
          )
          .map(({ id }) => id)
      : []

    sellItems(sellableUserItemsIds)
  }

  return (
    <div className={classes.inventoryLootActions}>
      <div className={classes.canSellFilter}>
        <span>Можно продать</span>

        <Switch name='canSellFilter' />
      </div>

      <Button color='purple'>Вывести</Button>

      <Button
        onClick={() => {
          setSellConfirmModalOpen(true)
        }}
      >
        Продать все
      </Button>

      <ConfirmModal
        open={isSellConfirmModalOpen}
        text='Вы уверены что хотите продать все предметы?'
        loading={isSellInProcess}
        onConfirm={handleAllItemsSell}
        onClose={() => {
          setSellConfirmModalOpen(false)
        }}
      />
    </div>
  )
}
